using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context.Interfaces
{
    public interface IDbContext : IDbContextTransactionOperations
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
    }
}