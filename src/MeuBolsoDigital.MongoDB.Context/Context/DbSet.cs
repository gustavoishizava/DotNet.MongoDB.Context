using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.Context
{
    public class DbSet<TModel> where TModel : class
    {
        public IMongoCollection<TModel> Collection { get; private set; }
        public DbContext DbContext { get; private set; }

        public DbSet(IMongoCollection<TModel> collection, DbContext dbContext)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection), "Collection cannot be null.");

            if (dbContext is null)
                throw new ArgumentNullException(nameof(dbContext), "DbContext cannot be null.");

            Collection = collection;
            DbContext = dbContext;
        }
    }
}