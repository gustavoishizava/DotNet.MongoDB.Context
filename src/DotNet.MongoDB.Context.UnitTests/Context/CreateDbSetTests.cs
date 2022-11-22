using System;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using DotNet.MongoDB.Context.UnitTests.Context.Common;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Context
{
    public class CreateDbSetTests
    {
        private class DbSetContextTest : DbContext
        {
            public DbSetContextTest(IMongoClient mongoClient, IMongoDatabase mongoDatabase, MongoDbContextOptions options) : base(mongoClient, mongoDatabase, options)
            {
            }
        }

        private Mock<IMongoClient> _mockMongoClient;
        private Mock<IMongoDatabase> _mockMongoDatabase;
        private Mock<IClientSessionHandle> _mockClientSessionHandle;
        private MongoDbContextOptions _contextOptions;

        public CreateDbSetTests()
        {
            _mockMongoClient = new Mock<IMongoClient>();
            _mockMongoDatabase = new Mock<IMongoDatabase>();
            _mockClientSessionHandle = new Mock<IClientSessionHandle>();
            _contextOptions = new MongoDbContextOptions("mongodb://tes123", "TestDB");

            _mockMongoClient.Setup(x => x.GetDatabase(_contextOptions.DatabaseName, null))
                .Returns(_mockMongoDatabase.Object);

            _mockMongoClient.Setup(x => x.StartSession(null, default))
                .Returns(_mockClientSessionHandle.Object);
        }

        private DbSetContextTest CreateContext()
        {
            return new DbSetContextTest(_mockMongoClient.Object, _mockMongoDatabase.Object, _contextOptions);
        }

        [Fact]
        public void MongoCollectionNull_Create_ReturnArgumentNullException()
        {
            // Arrange
            var dbContext = CreateContext();
            var exceptionMessage = "Collection cannot be null. (Parameter 'collection')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new DbSet<Product>(null, dbContext));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public void DbContextNull_Create_ReturnArgumentNullException()
        {
            // Arrange
            var collectionMock = new Mock<IMongoCollection<Product>>();
            var exceptionMessage = "DbContext cannot be null. (Parameter 'dbContext')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new DbSet<Product>(collectionMock.Object, null));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public void Create_ReturnSuccess()
        {
            // Arrange
            var dbContext = CreateContext();
            var collectionMock = new Mock<IMongoCollection<Product>>();

            // Act
            var dbSet = new DbSet<Product>(collectionMock.Object, dbContext);

            // Assert
            Assert.NotNull(dbSet.Collection);
            Assert.NotNull(dbSet.DbContext);
        }
    }
}