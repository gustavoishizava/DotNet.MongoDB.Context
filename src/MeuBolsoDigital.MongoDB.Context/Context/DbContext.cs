using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.Context.Interfaces;
using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context
{
    public abstract class DbContext : IDbContext, IDisposable
    {
        public IMongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        private IClientSessionHandle _clientSessionHandle { get; set; }

        public DbContext(MongoDbContextOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null.");

            Client = new MongoClient(options.ConnectionString);
            Database = Client.GetDatabase(options.DatabaseName);
        }

        public async Task<IClientSessionHandle> GetClientSessionHandleAsync()
        {
            if (_clientSessionHandle is null)
                return await Client.StartSessionAsync();

            return _clientSessionHandle;
        }

        public async Task StartTransactionAsync()
        {
            var session = await GetClientSessionHandleAsync();
            if (!session.IsInTransaction)
                session.StartTransaction();
        }

        public async Task CommitAsync()
        {
            if (_clientSessionHandle is not null && _clientSessionHandle.IsInTransaction)
                await _clientSessionHandle.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            if (_clientSessionHandle is not null && _clientSessionHandle.IsInTransaction)
                await _clientSessionHandle.AbortTransactionAsync();
        }

        public void Dispose()
        {
            _clientSessionHandle.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}