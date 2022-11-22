using System.Threading.Tasks;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using DotNet.MongoDB.Context.UnitTests.Context.Common;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Context
{
    public class TransactionDbContextTests
    {
        private class TransactionContextTest : DbContext
        {
            public DbSet<Product> Products { get; set; }

            public TransactionContextTest(IMongoClient mongoClient, IMongoDatabase mongoDatabase, MongoDbContextOptions options) : base(mongoClient, mongoDatabase, options)
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

            var mockProductCollection = new Mock<IMongoCollection<Product>>();
            _mockMongoDatabase.Setup(x => x.GetCollection<Product>(It.IsAny<string>(), null))
                .Returns(mockProductCollection.Object);
        }

        private TransactionContextTest CreateContext()
        {
            return new TransactionContextTest(_mockMongoClient.Object, _mockMongoDatabase.Object, _contextOptions);
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
            await context.Products.AddAsync(new Product());

            // Act
            await context.CommitAsync();

            // Assert
            _mockClientSessionHandle.Verify(x => x.IsInTransaction, Times.Once);
            _mockClientSessionHandle.Verify(x => x.CommitTransactionAsync(default), Times.Once);
            Assert.Empty(context.ChangeTracker.Entries);
        }

        [Fact]
        public async Task NotInTransaction_Commit_DoNothing()
        {
            // Arrange
            _mockClientSessionHandle.Setup(x => x.IsInTransaction)
                .Returns(false);

            var context = CreateContext();
            await context.Products.AddAsync(new Product());

            // Act
            await context.CommitAsync();

            // Assert
            _mockClientSessionHandle.Verify(x => x.IsInTransaction, Times.Once);
            _mockClientSessionHandle.Verify(x => x.CommitTransactionAsync(default), Times.Never);
            Assert.Empty(context.ChangeTracker.Entries);
        }

        [Fact]
        public async Task IsInTransaction_Rollback_ShouldRollbackTransaction()
        {
            // Arrange
            _mockClientSessionHandle.Setup(x => x.IsInTransaction)
                .Returns(true);

            var context = CreateContext();
            await context.Products.AddAsync(new Product());

            // Act
            await context.RollbackAsync();

            // Assert
            _mockClientSessionHandle.Verify(x => x.IsInTransaction, Times.Once);
            _mockClientSessionHandle.Verify(x => x.AbortTransactionAsync(default), Times.Once);
            Assert.Empty(context.ChangeTracker.Entries);
        }

        [Fact]
        public async Task NotInTransaction_Rollback_DoNothing()
        {
            // Arrange
            _mockClientSessionHandle.Setup(x => x.IsInTransaction)
                .Returns(false);

            var context = CreateContext();
            await context.Products.AddAsync(new Product());

            // Act
            await context.RollbackAsync();

            // Assert
            _mockClientSessionHandle.Verify(x => x.IsInTransaction, Times.Once);
            _mockClientSessionHandle.Verify(x => x.AbortTransactionAsync(default), Times.Never);
            Assert.Empty(context.ChangeTracker.Entries);
        }
    }
}