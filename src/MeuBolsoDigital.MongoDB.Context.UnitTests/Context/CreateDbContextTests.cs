using System;
using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.UnitTests.Context.Common;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace MeuBolsoDigital.MongoDB.Context.UnitTests.Context
{
    public class CreateDbContextTests
    {
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

            _mockMongoClient.Setup(x => x.GetDatabase(_contextOptions.DatabaseName, null))
                .Returns(_mockMongoDatabase.Object);

            _mockMongoClient.Setup(x => x.StartSession(null, default))
                .Returns(_mockClientSessionHandle.Object);

            var mockCollection = new Mock<IMongoCollection<Product>>();
            _mockMongoDatabase.Setup(x => x.GetCollection<Product>(It.IsAny<string>(), null))
                .Returns(mockCollection.Object);
        }

        private TestDbContext CreateContext()
        {
            return new TestDbContext(_mockMongoClient.Object, _contextOptions.DatabaseName);
        }

        [Fact]
        public void NullOptions_CreateDbContext_ReturnArgumentNullException()
        {
            // Arrange
            var exceptionMessage = "Options cannot be null. (Parameter 'options')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new TestDbContext(null));
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
            Assert.NotNull(context.Products);
            Assert.Single(context.ModelMaps);
        }
    }
}