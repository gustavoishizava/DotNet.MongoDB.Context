using MongoDB.Bson.Serialization;

namespace DotNet.MongoDB.Context.Mapping
{
    public abstract class BsonClassMapConfiguration : IBsonClassMapConfiguration
    {
        public string CollectionName { get; private init; }
        public bool IsEntity => !string.IsNullOrEmpty(CollectionName);
        public BsonClassMap BsonClassMap { get; private set; }

        protected BsonClassMapConfiguration(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException(nameof(collectionName), "Collection name cannot be null or empty.");

            CollectionName = collectionName;
        }

        protected BsonClassMapConfiguration() { }

        private bool MapExists(Type type) => BsonClassMap.IsClassMapRegistered(type);

        public void Apply()
        {
            BsonClassMap = GetConfiguration();

            if (BsonClassMap is null || MapExists(BsonClassMap.ClassType))
                return;

            BsonClassMap.RegisterClassMap(BsonClassMap);
        }

        public abstract BsonClassMap GetConfiguration();
    }
}