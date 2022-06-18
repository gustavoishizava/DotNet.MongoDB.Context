using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context.Interfaces
{
    public interface IDbContext
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
        Task<IClientSessionHandle> GetClientSessionHandleAsync();
        Task StartTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}