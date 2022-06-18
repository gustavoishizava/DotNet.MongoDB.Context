using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.Context.Interfaces;
using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context
{
    public abstract class DbContext : IDbContext, IDisposable
    {
        public IMongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        private IClientSessionHandle ClientSessionHandle { get; set; }

        public DbContext(MongoDbContextOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options), "Options cannot be null.");

            Client = new MongoClient(options.ConnectionString);
            Database = Client.GetDatabase(options.DatabaseName);
        }

        public async Task<IClientSessionHandle> GetClientSessionHandleAsync()
        {
            if (ClientSessionHandle is null)
            {
                var session = await Client.StartSessionAsync();
                session.StartTransaction();

                return session;
            }

            return ClientSessionHandle;
        }

        public async Task CommitAsync()
        {
            if (ClientSessionHandle is not null)
                await ClientSessionHandle.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            if (ClientSessionHandle is not null)
                await ClientSessionHandle.AbortTransactionAsync();
        }

        public void Dispose()
        {
            ClientSessionHandle.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}