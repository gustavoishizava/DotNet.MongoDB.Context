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
        private Dictionary<Type, string> _collections { get; set; }

        public DbContext(MongoDbContextOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null.");

            Client = new MongoClient(options.ConnectionString);
            Database = Client.GetDatabase(options.DatabaseName);
            _clientSessionHandle = Client.StartSession();
            _collections = ConfigureCollections();
        }

        protected abstract Dictionary<Type, string> ConfigureCollections();

        public IMongoCollection<TDocument> GetCollection<TDocument>()
        {
            if (!_collections.TryGetValue(typeof(TDocument), out var collectionName))
                throw new KeyNotFoundException($"Collection not found to type {typeof(TDocument).Name}.");

            return Database.GetCollection<TDocument>(collectionName);
        }

        #region Transaction operations

        public void StartTransaction()
        {
            if (!_clientSessionHandle.IsInTransaction)
                _clientSessionHandle.StartTransaction();
        }

        public async Task CommitAsync()
        {
            if (_clientSessionHandle is not null && _clientSessionHandle.IsInTransaction)
                await _clientSessionHandle.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            if (_clientSessionHandle is not null && _clientSessionHandle.IsInTransaction)
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
            return await GetCollection<TDocument>().Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<TDocument>> GetManyAsync<TDocument>(FilterDefinition<TDocument> filter)
        {
            return await GetCollection<TDocument>().Find(filter).ToListAsync();
        }

        #endregion

        public void Dispose()
        {
            _clientSessionHandle.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}