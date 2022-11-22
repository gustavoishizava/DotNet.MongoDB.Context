using MongoDB.Bson.Serialization;

namespace DotNet.MongoDB.Context.Mapping
{
    public abstract class BsonClassMapConfiguration<TModel> : IBsonClassMapConfiguration<TModel>
    {
        public string CollectionName { get; private init; }
        public bool IsEntity => !string.IsNullOrEmpty(CollectionName);

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
            var bsonClassMap = GetConfiguration();

            if (bsonClassMap is null || MapExists(typeof(TModel)))
                return;

            BsonClassMap.RegisterClassMap(bsonClassMap);
        }

        public abstract BsonClassMap<TModel> GetConfiguration();
    }
}