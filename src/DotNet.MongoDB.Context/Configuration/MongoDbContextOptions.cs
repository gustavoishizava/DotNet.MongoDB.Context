using DotNet.MongoDB.Context.Mapping;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace DotNet.MongoDB.Context.Configuration
{
    public class MongoDbContextOptions
    {
        private readonly List<IBsonSerializer> _serializers;
        private readonly List<IBsonClassMapConfiguration> _bsonClassMapConfigurations;
        internal readonly ConventionPack ConventionPack;
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }

        public IReadOnlyList<IConvention> Conventions => ConventionPack.Conventions.ToList().AsReadOnly();
        public IReadOnlyCollection<IBsonSerializer> Serializers => _serializers.AsReadOnly();
        public IReadOnlyCollection<IBsonClassMapConfiguration> BsonClassMapConfigurations => _bsonClassMapConfigurations.AsReadOnly();

        public MongoDbContextOptions(string connectionString, string databaseName) : this()
        {
            ConfigureConnection(connectionString, databaseName);
        }

        internal MongoDbContextOptions()
        {
            ConventionPack = new();
            _serializers = new();
            _bsonClassMapConfigurations = new();
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

        public void AddConvention(IConvention convention)
        {
            ConventionPack.Add(convention);
        }

        public void AddSerializer(IBsonSerializer bsonSerializer)
        {
            _serializers.Add(bsonSerializer);
        }

        public void AddBsonClassMap(IBsonClassMapConfiguration bsonClassMapConfiguration)
        {
            _bsonClassMapConfigurations.Add(bsonClassMapConfiguration);
        }
    }
}