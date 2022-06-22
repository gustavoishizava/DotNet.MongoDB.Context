using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context.Interfaces
{
    internal interface IDbSet<TDocument> where TDocument : class
    {
        IMongoCollection<TDocument> Collection { get; }
        DbContext DbContext { get; }

        Task AddAsync(TDocument document);
        Task AddRangeAsync(List<TDocument> documents);
        Task UpdateAsync(FilterDefinition<TDocument> filter, TDocument document);
        Task UpdateManyAsync(FilterDefinition<TDocument> filter, TDocument document);
        Task RemoveAsync(FilterDefinition<TDocument> filter);
        Task RemoveRangeAsync(FilterDefinition<TDocument> filter);
    }
}