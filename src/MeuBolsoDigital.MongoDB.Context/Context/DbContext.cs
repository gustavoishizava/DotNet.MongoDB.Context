using System.Reflection;
using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.Context.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context
{
    public abstract class DbContext : IDbContext, IDisposable
    {
        public IMongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        private IClientSessionHandle _clientSessionHandle { get; set; }
        private Dictionary<Type, string> _collectionNames { get; init; }

        public IReadOnlyDictionary<Type, string> CollectionNames => _collectionNames;

        public DbContext(MongoDbContextOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null.");

            Client = new MongoClient(options.ConnectionString);
            Database = Client.GetDatabase(options.DatabaseName);
            _clientSessionHandle = Client.StartSession();
            _collectionNames = new Dictionary<Type, string>();

            ConfigureCollectionNames();
            OnModelConfiguring();
            RegisterCollections();
        }

        protected DbContext(IMongoClient mongoClient, string databaseName)
        {
            Client = mongoClient;
            Database = Client.GetDatabase(databaseName);
            _clientSessionHandle = Client.StartSession();
            _collectionNames = new Dictionary<Type, string>();

            ConfigureCollectionNames();
            OnModelConfiguring();
            RegisterCollections();
        }

        protected virtual void OnModelConfiguring()
        {
        }

        public abstract void OnModelNameConfiguring(Dictionary<Type, string> collectionNames);

        private void ConfigureCollectionNames()
        {
            var method = GetType().GetMethod(nameof(DbContext.OnModelNameConfiguring));
            method.Invoke(this, new object[] { _collectionNames });
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

                var collectionName = _collectionNames.TryGetValue(documentType, out var name) ? name : documentType.Name;

                var mongoCollection = getCollectionMethod.Invoke(Database, new object[] { collectionName, null });
                property.SetValue(this, mongoCollection);
            }
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>()
        {
            return (IMongoCollection<TDocument>)GetCollectionProperties()
                .First(x => x.PropertyType == typeof(IMongoCollection<TDocument>))
                .GetValue(this);
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

        #region Collection Operations

        public async Task InsertOneAsync<TDocument>(TDocument document)
        {
            StartTransaction();
            await GetCollection<TDocument>().InsertOneAsync(_clientSessionHandle, document);
        }

        public async Task InsertManyAsync<TDocument>(List<TDocument> documents)
        {
            StartTransaction();
            await GetCollection<TDocument>().InsertManyAsync(_clientSessionHandle, documents);
        }

        public async Task UpdateAsync<TDocument>(FilterDefinition<TDocument> filter, TDocument document)
        {
            StartTransaction();
            var update = new BsonDocument { { "$set", document.ToBsonDocument() } };
            await GetCollection<TDocument>().UpdateOneAsync(_clientSessionHandle, filter, update, new() { IsUpsert = true });
        }

        public async Task UpdateManyAsync<TDocument>(FilterDefinition<TDocument> filter, TDocument document)
        {
            StartTransaction();
            var update = new BsonDocument { { "$set", document.ToBsonDocument() } };
            await GetCollection<TDocument>().UpdateManyAsync(_clientSessionHandle, filter, update, new() { IsUpsert = true });
        }

        public async Task RemoveAsync<TDocument>(FilterDefinition<TDocument> filter)
        {
            StartTransaction();
            await GetCollection<TDocument>().DeleteOneAsync(_clientSessionHandle, filter);
        }

        public async Task RemoveManyAsync<TDocument>(FilterDefinition<TDocument> filter)
        {
            StartTransaction();
            await GetCollection<TDocument>().DeleteManyAsync(_clientSessionHandle, filter);
        }

        public async Task<TDocument> GetOneAsync<TDocument>(FilterDefinition<TDocument> filter)
        {
            return await GetCollection<TDocument>().Find(_clientSessionHandle, filter).FirstOrDefaultAsync();
        }

        public async Task<List<TDocument>> GetManyAsync<TDocument>(FilterDefinition<TDocument> filter)
        {
            return await GetCollection<TDocument>().Find(_clientSessionHandle, filter).ToListAsync();
        }

        #endregion

        public void Dispose()
        {
            _clientSessionHandle.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}