using System.Threading.Tasks;
using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.Context;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace MeuBolsoDigital.MongoDB.Context.UnitTests.Context
{
    public class TransactionDbContextTests
    {
        private class TransactionContextTest : DbContext
        {
            public TransactionContextTest(MongoDbContextOptions options) : base(options)
            {
            }

            public TransactionContextTest(IMongoClient mongoClient, string databaseName) : base(mongoClient, databaseName)
            {
            }
        }

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

        private TransactionContextTest CreateContext()
        {
            return new TransactionContextTest(_mockMongoClient.Object, _contextOptions.DatabaseName);
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

        [Fact]
        public async Task IsInTransaction_Commit_ShouldCommitTransaction()
        {
            // Arrange
            _mockClientSessionHandle.Setup(x => x.IsInTransaction)
                .Returns(true);

            var context = CreateContext();

            // Act
            await context.CommitAsync();

            // Assert
            _mockClientSessionHandle.Verify(x => x.IsInTransaction, Times.Once);
            _mockClientSessionHandle.Verify(x => x.CommitTransactionAsync(default), Times.Once);
        }

        [Fact]
        public async Task NotInTransaction_Commit_DoNothing()
        {
            // Arrange
            _mockClientSessionHandle.Setup(x => x.IsInTransaction)
                .Returns(false);

            var context = CreateContext();

            // Act
            await context.CommitAsync();

            // Assert
            _mockClientSessionHandle.Verify(x => x.IsInTransaction, Times.Once);
            _mockClientSessionHandle.Verify(x => x.CommitTransactionAsync(default), Times.Never);
        }

        [Fact]
        public async Task IsInTransaction_Rollback_ShouldRollbackTransaction()
        {
            // Arrange
            _mockClientSessionHandle.Setup(x => x.IsInTransaction)
                .Returns(true);

            var context = CreateContext();

            // Act
            await context.RollbackAsync();

            // Assert
            _mockClientSessionHandle.Verify(x => x.IsInTransaction, Times.Once);
            _mockClientSessionHandle.Verify(x => x.AbortTransactionAsync(default), Times.Once);
        }

        [Fact]
        public async Task NotInTransaction_Rollback_DoNothing()
        {
            // Arrange
            _mockClientSessionHandle.Setup(x => x.IsInTransaction)
                .Returns(false);

            var context = CreateContext();

            // Act
            await context.RollbackAsync();

            // Assert
            _mockClientSessionHandle.Verify(x => x.IsInTransaction, Times.Once);
            _mockClientSessionHandle.Verify(x => x.AbortTransactionAsync(default), Times.Never);
        }
    }
}