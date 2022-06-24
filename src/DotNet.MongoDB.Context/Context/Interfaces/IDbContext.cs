using DotNet.MongoDB.Context.Context.ChangeTracking;
using MongoDB.Driver;

namespace DotNet.MongoDB.Context.Context.Interfaces
{
    internal interface IDbContext : IDbContextTransactionOperations
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
        IClientSessionHandle ClientSessionHandle { get; }
        ChangeTracker ChangeTracker { get; }
    }
}