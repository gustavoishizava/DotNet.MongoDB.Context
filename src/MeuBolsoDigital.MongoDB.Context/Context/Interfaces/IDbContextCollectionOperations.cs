using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context.Interfaces
{
    public interface IDbContextCollectionOperations
    {
        Task InsertOneAsync<TDocument>(TDocument document);
        Task InsertManyAsync<TDocument>(List<TDocument> documents);
        Task UpdateAsync<TDocument>(FilterDefinition<TDocument> filter, TDocument document);
        Task UpdateManyAsync<TDocument>(FilterDefinition<TDocument> filter, TDocument document);
        Task RemoveAsync<TDocument>(FilterDefinition<TDocument> filter);
        Task RemoveManyAsync<TDocument>(FilterDefinition<TDocument> filter);
        Task<TDocument> GetOneAsync<TDocument>(FilterDefinition<TDocument> filter);
        Task<List<TDocument>> GetManyAsync<TDocument>(FilterDefinition<TDocument> filter);
    }
}