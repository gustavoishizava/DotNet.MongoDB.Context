using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.Context;
using MeuBolsoDigital.MongoDB.Context.Context.ModelConfiguration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

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

    public class TestDbContext : DbContext
    {
        public IMongoCollection<Product> Products { get; set; }
        public IMongoCollection<Customer> Customers { get; set; }
        public IMongoCollection<User> Users { get; set; }

        public TestDbContext(MongoDbContextOptions options) : base(options)
        {
        }

        public TestDbContext(IMongoClient mongoClient, string databaseName) : base(mongoClient, databaseName)
        {
        }

        protected override void OnModelConfiguring(ModelBuilder modelBuilder)
        {
            modelBuilder.AddModelMap("customers", new BsonClassMap<Customer>());
        }
    }
}