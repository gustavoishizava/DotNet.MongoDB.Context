using System.Reflection;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context.ChangeTracking;
using DotNet.MongoDB.Context.Context.Interfaces;
using DotNet.MongoDB.Context.Context.ModelConfiguration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace DotNet.MongoDB.Context.Context
{
    public abstract class DbContext : IDbContext, IDisposable
    {
        private readonly ModelBuilder _modelBuilder = new();
        public IMongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public IClientSessionHandle ClientSessionHandle { get; private set; }
        public ChangeTracker ChangeTracker { get; private set; }

        public IReadOnlyList<ModelMap> ModelMaps => _modelBuilder.ModelMaps;

        protected DbContext(MongoDbContextOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null.");

            ApplyConventions(options.ConventionPack);
            ApplySerializer(options.Serializers.ToList());

            Client = new MongoClient(options.ConnectionString);
            Configure(options.DatabaseName);
        }

        protected DbContext(IMongoClient mongoClient, string databaseName, MongoDbContextOptions options = null)
        {
            if (options is not null)
            {
                ApplyConventions(options.ConventionPack);
                ApplySerializer(options.Serializers.ToList());
            }

            Client = mongoClient;
            Configure(databaseName);
        }

        private void ApplyConventions(ConventionPack conventionPack)
        {
            ConventionRegistry.Register("Conventions", conventionPack, _ => true);
        }

        private void ApplySerializer(List<IBsonSerializer> serializers)
        {
            serializers.ForEach(x =>
            {
                var type = x.GetType().BaseType.GenericTypeArguments[0];
                BsonSerializer.RegisterSerializer(type, x);
            });
        }

        private void Configure(string databaseName)
        {
            Database = Client.GetDatabase(databaseName);
            ClientSessionHandle = Client.StartSession();
            ChangeTracker = new();

            ConfigureModels();
            RegisterCollections();
        }

        protected virtual void OnModelConfiguring(ModelBuilder modelBuilder)
        {
        }

        private void ConfigureModels()
        {
            var method = GetType().GetMethod(nameof(DbContext.OnModelConfiguring), BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(this, new object[] { _modelBuilder });

            foreach (var map in _modelBuilder.ModelMaps)
            {
                if (!BsonClassMap.IsClassMapRegistered(map.BsonClassMap.ClassType))
                    BsonClassMap.RegisterClassMap(map.BsonClassMap);
            }
        }

        private List<PropertyInfo> GetCollectionProperties()
        {
            return GetType().GetProperties()
                    .Where(x => x.PropertyType.IsClass
                                && x.PropertyType.IsGenericType
                                && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).ToList();
        }

        private void RegisterCollections()
        {
            var collectionProperties = GetCollectionProperties();

            foreach (var property in collectionProperties)
            {
                var documentType = property.PropertyType.GenericTypeArguments[0];

                var getCollectionMethod = Database.GetType().GetMethod(nameof(IMongoDatabase.GetCollection))
                                            .MakeGenericMethod(new[] { documentType });

                var collectionName = _modelBuilder.GetCollectionName(documentType) ?? documentType.Name;
                var mongoCollection = getCollectionMethod.Invoke(Database, new object[] { collectionName, null });
                var dbSetType = typeof(DbSet<>).MakeGenericType(new[] { documentType });

                property.SetValue(this, Activator.CreateInstance(dbSetType, new object[] { mongoCollection, this }));
            }
        }

        public void StartTransaction()
        {
            if (!ClientSessionHandle.IsInTransaction)
                ClientSessionHandle.StartTransaction();
        }

        public virtual async Task CommitAsync()
        {
            if (ClientSessionHandle.IsInTransaction)
                await ClientSessionHandle.CommitTransactionAsync();

            ChangeTracker.Clear();
        }

        public async Task RollbackAsync()
        {
            if (ClientSessionHandle.IsInTransaction)
                await ClientSessionHandle.AbortTransactionAsync();

            ChangeTracker.Clear();
        }

        public void Dispose()
        {
            ClientSessionHandle.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}