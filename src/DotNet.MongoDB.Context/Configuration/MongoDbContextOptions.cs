using DotNet.MongoDB.Context.Mapping;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace DotNet.MongoDB.Context.Configuration
{
    public class MongoDbContextOptions
    {
        public readonly List<Serializer> Serializers;
        public readonly List<IBsonClassMapConfiguration> BsonClassMapConfigurations;
        internal readonly ConventionPack ConventionPack;
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }

        public IReadOnlyList<IConvention> Conventions => ConventionPack.Conventions.ToList().AsReadOnly();

        public MongoDbContextOptions(string connectionString, string databaseName) : this()
        {
            ConfigureConnection(connectionString, databaseName);
        }

        internal MongoDbContextOptions()
        {
            ConventionPack = new();
            Serializers = new();
            BsonClassMapConfigurations = new();
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
            Serializers.Add(new Serializer(bsonSerializer));
        }

        public void AddBsonClassMap(IBsonClassMapConfiguration bsonClassMapConfiguration)
        {
            BsonClassMapConfigurations.Add(bsonClassMapConfiguration);
        }
    }
}