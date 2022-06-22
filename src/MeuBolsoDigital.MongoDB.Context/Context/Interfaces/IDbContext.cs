using MeuBolsoDigital.MongoDB.Context.Context.ChangeTracking;
using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context.Interfaces
{
    internal interface IDbContext : IDbContextTransactionOperations
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
        IClientSessionHandle ClientSessionHandle { get; }
        ChangeTracker ChangeTracker { get; }
    }
}