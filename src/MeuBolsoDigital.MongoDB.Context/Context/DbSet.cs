using MeuBolsoDigital.MongoDB.Context.Context.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context
{
    public class DbSet<TDocument> : IDbSet<TDocument> where TDocument : class
    {
        public IMongoCollection<TDocument> Collection { get; private set; }
        public DbContext DbContext { get; private set; }

        public DbSet(IMongoCollection<TDocument> collection, DbContext dbContext)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection), "Collection cannot be null.");

            if (dbContext is null)
                throw new ArgumentNullException(nameof(dbContext), "DbContext cannot be null.");

            Collection = collection;
            DbContext = dbContext;
        }

        public async Task AddAsync(TDocument document)
        {
            await Collection.InsertOneAsync(DbContext.ClientSessionHandle, document);
        }

        public async Task AddRangeAsync(List<TDocument> documents)
        {
            await Collection.InsertManyAsync(DbContext.ClientSessionHandle, documents);
        }

        public async Task UpdateAsync(FilterDefinition<TDocument> filter, TDocument document)
        {
            var update = new BsonDocument { { "$set", document.ToBsonDocument() } };
            await Collection.UpdateOneAsync(DbContext.ClientSessionHandle, filter, update, new() { IsUpsert = true });
        }

        public async Task UpdateManyAsync(FilterDefinition<TDocument> filter, TDocument document)
        {
            var update = new BsonDocument { { "$set", document.ToBsonDocument() } };
            await Collection.UpdateManyAsync(DbContext.ClientSessionHandle, filter, update, new() { IsUpsert = true });
        }

        public async Task RemoveAsync(FilterDefinition<TDocument> filter)
        {
            await Collection.DeleteOneAsync(DbContext.ClientSessionHandle, filter);
        }

        public async Task RemoveRangeAsync(FilterDefinition<TDocument> filter)
        {
            await Collection.DeleteManyAsync(DbContext.ClientSessionHandle, filter);
        }
    }
}