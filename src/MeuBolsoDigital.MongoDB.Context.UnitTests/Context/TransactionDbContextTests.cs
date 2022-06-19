using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.UnitTests.Context.Common;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace MeuBolsoDigital.MongoDB.Context.UnitTests.Context
{
    public class TransactionDbContextTests
    {
        private Mock<IMongoClient> _mockMongoClient;
        private Mock<IMongoDatabase> _mockMongoDatabase;
        private Mock<IClientSessionHandle> _mockClientSessionHandle;
        private MongoDbContextOptions _contextOptions;

        public TransactionDbContextTests()
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

        private TestDbContext CreateContext()
        {
            return new TestDbContext(_mockMongoClient.Object, _contextOptions.DatabaseName);
        }

        [Fact]
        public void NoTransaction_StartTransaction_ReturnSuccess()
        {
            // Arrange
            _mockClientSessionHandle.Setup(x => x.IsInTransaction)
                .Returns(false);

            var context = CreateContext();

            // Act
            context.StartTransaction();

            // Assert
            _mockClientSessionHandle.Verify(x => x.IsInTransaction, Times.Once);
            _mockClientSessionHandle.Verify(x => x.StartTransaction(null), Times.Once);
        }

        [Fact]
        public void HasTransaction_StartTransaction_DoNothing()
        {
            // Arrange
            _mockClientSessionHandle.Setup(x => x.IsInTransaction)
                .Returns(true);

            var context = CreateContext();

            // Act
            context.StartTransaction();

            // Assert
            _mockClientSessionHandle.Verify(x => x.IsInTransaction, Times.Once);
            _mockClientSessionHandle.Verify(x => x.StartTransaction(null), Times.Never);
        }
    }
}