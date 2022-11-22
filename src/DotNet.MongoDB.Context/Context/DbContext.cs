using System.Reflection;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context.ChangeTracking;
using DotNet.MongoDB.Context.Context.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace DotNet.MongoDB.Context.Context
{
    public abstract class DbContext : IDbContext, IDisposable
    {
        public IMongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public IClientSessionHandle ClientSessionHandle { get; private set; }
        public ChangeTracker ChangeTracker { get; private set; }
        private readonly MongoDbContextOptions _options;

        protected DbContext(IMongoClient mongoClient, IMongoDatabase mongoDatabase, MongoDbContextOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null.");

            _options = options;

            ApplyConventions(options.ConventionPack);
            ApplySerializer(options.Serializers);

            Client = mongoClient;
            Database = mongoDatabase;

            ClientSessionHandle = Client.StartSession();
            ChangeTracker = new();

            RegisterCollections();
        }

        private void ApplyConventions(ConventionPack conventionPack)
        {
            ConventionRegistry.Register("Conventions", conventionPack, _ => true);
        }

        private void ApplySerializer(List<Serializer> serializers)
        {
            serializers.ForEach(x =>
            {
                if (x.Registered)
                    return;

                var type = x.BsonSerializer.GetType().BaseType.GenericTypeArguments[0];
                BsonSerializer.RegisterSerializer(type, x.BsonSerializer);
                x.Register();
            });
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

                var mapping = _options.BsonClassMapConfigurations.FirstOrDefault(x => x.BsonClassMap.ClassType == documentType);
                var collectionName = mapping?.CollectionName ?? documentType.Name;
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