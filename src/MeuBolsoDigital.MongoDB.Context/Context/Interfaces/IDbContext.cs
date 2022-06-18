using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context.Interfaces
{
    public interface IDbContext : IDbContextTransactionOperations, IDbContextCollectionOperations
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
        IMongoCollection<TDocument> GetCollection<TDocument>();
    }
}