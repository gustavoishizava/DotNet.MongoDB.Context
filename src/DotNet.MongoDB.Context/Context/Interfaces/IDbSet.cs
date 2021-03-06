using DotNet.MongoDB.Context.Context.Operations;
using MongoDB.Driver;

namespace DotNet.MongoDB.Context.Context.Interfaces
{
    internal interface IDbSet<TDocument> where TDocument : class
    {
        IMongoCollection<TDocument> Collection { get; }
        DbContext DbContext { get; }

        Task AddAsync(TDocument document);
        Task AddRangeAsync(List<TDocument> documents);
        Task UpdateAsync(FilterDefinition<TDocument> filter, TDocument document);
        Task UpdateRangeAsync(List<BulkOperationModel<TDocument>> bulkOperationModels);
        Task RemoveAsync(FilterDefinition<TDocument> filter, TDocument document);
        Task RemoveRangeAsync(List<BulkOperationModel<TDocument>> bulkOperationModels);
    }
}