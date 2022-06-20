using MongoDB.Bson.Serialization;

namespace MeuBolsoDigital.MongoDB.Context.Context.ModelConfiguration
{
    public class ModelMap
    {
        public string CollectionName { get; private init; }
        public BsonClassMap BsonClassMap { get; private init; }

        protected ModelMap()
        {
        }

        internal ModelMap(string collectionName, BsonClassMap bsonClassMap)
        {
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException(nameof(collectionName), "Collection name cannot be null.");

            if (bsonClassMap is null)
                throw new ArgumentNullException(nameof(bsonClassMap), "BsonClassMap cannot be null.");

            CollectionName = collectionName;
            BsonClassMap = bsonClassMap;
        }
    }
}