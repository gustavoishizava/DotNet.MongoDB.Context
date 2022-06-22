using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MeuBolsoDigital.MongoDB.Context.UnitTests.Context.Common
{
    public class Product
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }

    public class Customer
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }

    public class User : Customer
    {
    }
}