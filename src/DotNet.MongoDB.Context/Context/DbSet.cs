using DotNet.MongoDB.Context.Context.ChangeTracking;
using DotNet.MongoDB.Context.Context.Interfaces;
using DotNet.MongoDB.Context.Context.Operations;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DotNet.MongoDB.Context.Context
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
            DbContext.ChangeTracker.AddEntry(new(EntryState.Added, document));

            await Collection.InsertOneAsync(DbContext.ClientSessionHandle, document);
        }

        public async Task AddRangeAsync(List<TDocument> documents)
        {
            documents.ForEach(x => DbContext.ChangeTracker.AddEntry(new(EntryState.Added, x)));

            await Collection.InsertManyAsync(DbContext.ClientSessionHandle, documents);
        }

        public async Task UpdateAsync(FilterDefinition<TDocument> filter, TDocument document)
        {
            DbContext.ChangeTracker.AddEntry(new(EntryState.Modified, document));

            var update = new BsonDocument { { "$set", document.ToBsonDocument() } };
            await Collection.UpdateOneAsync(DbContext.ClientSessionHandle, filter, update, new() { IsUpsert = true });
        }

        public async Task UpdateRangeAsync(List<BulkOperationModel<TDocument>> bulkOperationModels)
        {
            bulkOperationModels.ForEach(x => DbContext.ChangeTracker.AddEntry(new(EntryState.Modified, x.Document)));

            var listWrites = bulkOperationModels.Select(x => new UpdateOneModel<TDocument>(x.Filter, new BsonDocument { { "$set", x.Document.ToBsonDocument() } }) { IsUpsert = true });
            await Collection.BulkWriteAsync(DbContext.ClientSessionHandle, listWrites, new() { IsOrdered = true });
        }

        public async Task RemoveAsync(FilterDefinition<TDocument> filter, TDocument document)
        {
            DbContext.ChangeTracker.AddEntry(new(EntryState.Deleted, document));

            await Collection.DeleteOneAsync(DbContext.ClientSessionHandle, filter);
        }

        public async Task RemoveRangeAsync(List<BulkOperationModel<TDocument>> bulkOperationModels)
        {
            bulkOperationModels.ForEach(x => DbContext.ChangeTracker.AddEntry(new(EntryState.Deleted, x.Document)));

            var listWrites = bulkOperationModels.Select(x => new DeleteOneModel<TDocument>(x.Filter));
            await Collection.BulkWriteAsync(DbContext.ClientSessionHandle, listWrites, new() { IsOrdered = true });
        }
    }
}