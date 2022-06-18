using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context.Interfaces
{
    public interface IDbContext
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
        void StartTransaction();
        Task CommitAsync();
        Task RollbackAsync();
    }
}