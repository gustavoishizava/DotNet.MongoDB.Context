using System;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using DotNet.MongoDB.Context.UnitTests.Context.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Context
{
    public class CreateDbContextTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<Product> Products { get; set; }
            public DbSet<Customer> Customers { get; set; }
            public DbSet<User> Users { get; set; }

            public TestDbContext(IMongoClient mongoClient, IMongoDatabase mongoDatabase, MongoDbContextOptions options) : base(mongoClient, mongoDatabase, options)
            {
            }
        }

        private Mock<IMongoClient> _mockMongoClient;
        private Mock<IMongoDatabase> _mockMongoDatabase;
        private Mock<IClientSessionHandle> _mockClientSessionHandle;
        private MongoDbContextOptions _contextOptions;

        public CreateDbContextTests()
        {
            _mockMongoClient = new Mock<IMongoClient>();
            _mockMongoDatabase = new Mock<IMongoDatabase>();
            _mockClientSessionHandle = new Mock<IClientSessionHandle>();
            _contextOptions = new MongoDbContextOptions("mongodb://tes123", "TestDB");

            _contextOptions.AddConvention(new CamelCaseElementNameConvention());
            _contextOptions.AddSerializer(new GuidSerializer(BsonType.String));

            _mockMongoClient.Setup(x => x.GetDatabase(_contextOptions.DatabaseName, null))
                .Returns(_mockMongoDatabase.Object);

            _mockMongoClient.Setup(x => x.StartSession(null, default))
                .Returns(_mockClientSessionHandle.Object);

            var mockProductCollection = new Mock<IMongoCollection<Product>>();
            _mockMongoDatabase.Setup(x => x.GetCollection<Product>(It.IsAny<string>(), null))
                .Returns(mockProductCollection.Object);

            var mockCustomerCollection = new Mock<IMongoCollection<Customer>>();
            _mockMongoDatabase.Setup(x => x.GetCollection<Customer>(It.IsAny<string>(), null))
                .Returns(mockCustomerCollection.Object);

            var mockUserCollection = new Mock<IMongoCollection<User>>();
            _mockMongoDatabase.Setup(x => x.GetCollection<User>(It.IsAny<string>(), null))
                .Returns(mockUserCollection.Object);
        }

        private TestDbContext CreateContext()
        {
            return new TestDbContext(_mockMongoClient.Object, _mockMongoDatabase.Object, _contextOptions);
        }

        [Fact]
        public void NullOptions_CreateDbContext_ReturnArgumentNullException()
        {
            // Arrange
            var exceptionMessage = "Options cannot be null. (Parameter 'options')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new TestDbContext(_mockMongoClient.Object, _mockMongoDatabase.Object, null));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public void WithOptions_CreateDbContext_ReturnSuccess()
        {
            // Act
            var context = CreateContext();

            // Assert
            Assert.NotNull(context.Client);
            Assert.NotNull(context.Database);
            Assert.NotNull(context.ClientSessionHandle);
            Assert.NotNull(context.ChangeTracker);
            Assert.NotNull(context.Products);
            Assert.NotNull(context.Customers);
            Assert.NotNull(context.Users);
        }
    }
}