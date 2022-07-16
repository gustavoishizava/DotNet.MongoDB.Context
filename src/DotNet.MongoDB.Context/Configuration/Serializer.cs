using MongoDB.Bson.Serialization;

namespace DotNet.MongoDB.Context.Configuration
{
    public class Serializer
    {
        public IBsonSerializer BsonSerializer { get; private set; }
        public bool Registered { get; private set; }

        public Serializer(IBsonSerializer bsonSerializer)
        {
            if (bsonSerializer is null)
                throw new ArgumentNullException(nameof(bsonSerializer), "BsonSerializer cannot be null.");

            BsonSerializer = bsonSerializer;
            Registered = false;
        }

        public void Register() => Registered = true;
    }
}