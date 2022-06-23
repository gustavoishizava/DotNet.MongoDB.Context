using System.Reflection;
using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.Context.ChangeTracking;
using MeuBolsoDigital.MongoDB.Context.Context.Interfaces;
using MeuBolsoDigital.MongoDB.Context.Context.ModelConfiguration;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context
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

            Client = new MongoClient(options.ConnectionString);
            Configure(options.DatabaseName);
        }

        protected DbContext(IMongoClient mongoClient, string databaseName)
        {
            Client = mongoClient;
            Configure(databaseName);
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