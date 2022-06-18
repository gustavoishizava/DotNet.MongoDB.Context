namespace MeuBolsoDigital.MongoDB.Context.Configuration
{
    public class MongoDbContextOptions
    {
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }

        public MongoDbContextOptions(string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null.");

            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException(nameof(databaseName), "Database name cannot be null.");

            ConnectionString = connectionString;
            DatabaseName = databaseName;
        }
    }
}