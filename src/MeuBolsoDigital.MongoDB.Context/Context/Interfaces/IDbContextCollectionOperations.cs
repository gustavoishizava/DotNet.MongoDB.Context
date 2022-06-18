namespace MeuBolsoDigital.MongoDB.Context.Context.Interfaces
{
    public interface IDbContextCollectionOperations
    {
        Task AddAsync<TDocument>(TDocument document);
        Task AddRangeAsync<TDocument>(List<TDocument> documents);
    }
}