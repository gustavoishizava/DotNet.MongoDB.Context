using System;
using System.Collections.Generic;
using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.Context;
using MongoDB.Driver;

namespace MeuBolsoDigital.MongoDB.Context.UnitTests.Context.Common
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(MongoDbContextOptions options) : base(options)
        {
        }

        public TestDbContext(IMongoClient mongoClient, string databaseName) : base(mongoClient, databaseName)
        {
        }

        protected override Dictionary<Type, string> ConfigureCollections()
        {
            return new Dictionary<Type, string>();
        }
    }
}