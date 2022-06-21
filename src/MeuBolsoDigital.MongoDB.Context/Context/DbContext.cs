using System.Reflection;
using MeuBolsoDigital.MongoDB.Context.Configuration;
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
        private IClientSessionHandle _clientSessionHandle { get; set; }

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
            _clientSessionHandle = Client.StartSession();

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
                    .Where(x => x.PropertyType.IsInterface
                                && x.PropertyType.IsGenericType
                                && x.PropertyType.GetGenericTypeDefinition() == typeof(IMongoCollection<>)).ToList();
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
                property.SetValue(this, mongoCollection);
            }
        }

        #region Transaction operations

        public void StartTransaction()
        {
            if (!_clientSessionHandle.IsInTransaction)
                _clientSessionHandle.StartTransaction();
        }

        public async Task CommitAsync()
        {
            if (_clientSessionHandle.IsInTransaction)
                await _clientSessionHandle.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            if (_clientSessionHandle.IsInTransaction)
                await _clientSessionHandle.AbortTransactionAsync();
        }

        #endregion

        public void Dispose()
        {
            _clientSessionHandle.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}