using System;
using System.Collections.Generic;
using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.Context;
using MongoDB.Bson;
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

    public class TestDbContext : DbContext
    {
        public IMongoCollection<Product> Products { get; set; }

        public TestDbContext(MongoDbContextOptions options) : base(options)
        {
        }

        public TestDbContext(IMongoClient mongoClient, string databaseName) : base(mongoClient, databaseName)
        {
        }

        public override void OnModelNameConfiguring(Dictionary<Type, string> collectionNames)
        {
            collectionNames.Add(typeof(Product), "products");
        }
    }
}