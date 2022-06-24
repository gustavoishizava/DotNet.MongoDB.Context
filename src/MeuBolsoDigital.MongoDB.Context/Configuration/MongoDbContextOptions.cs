namespace MeuBolsoDigital.MongoDB.Context.Configuration
{
    public class MongoDbContextOptions
    {
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }

        public MongoDbContextOptions(string connectionString, string databaseName)
        {
            ConfigureConnection(connectionString, databaseName);
        }

        internal MongoDbContextOptions()
        {
        }

        public void ConfigureConnection(string connectionString, string databaseName)
        {
            SetConnectionString(connectionString);
            SetDatabaseName(databaseName);
        }

        private void SetConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null.");

            ConnectionString = connectionString;
        }

        private void SetDatabaseName(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException(nameof(databaseName), "Database name cannot be null.");

            DatabaseName = databaseName;
        }
    }
}