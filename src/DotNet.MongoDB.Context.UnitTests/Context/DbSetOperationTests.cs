using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using DotNet.MongoDB.Context.Context.ChangeTracking;
using DotNet.MongoDB.Context.Context.Operations;
using DotNet.MongoDB.Context.UnitTests.Context.Common;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Context
{
    public class DbSetOperationTests
    {
        private class DbSetOperationContextTest : DbContext
        {
            public DbSetOperationContextTest(IMongoClient mongoClient, IMongoDatabase mongoDatabase, MongoDbContextOptions options) : base(mongoClient, mongoDatabase, options)
            {
            }
        }

        private Mock<IMongoClient> _mockMongoClient;
        private Mock<IMongoDatabase> _mockMongoDatabase;
        private Mock<IClientSessionHandle> _mockClientSessionHandle;
        private Mock<IMongoCollection<Product>> _mockCollection;
        private MongoDbContextOptions _contextOptions;

        public DbSetOperationTests()
        {
            _mockMongoClient = new Mock<IMongoClient>();
            _mockMongoDatabase = new Mock<IMongoDatabase>();
            _mockClientSessionHandle = new Mock<IClientSessionHandle>();
            _mockCollection = new Mock<IMongoCollection<Product>>();
            _contextOptions = new MongoDbContextOptions("mongodb://tes123", "TestDB");

            _mockMongoClient.Setup(x => x.GetDatabase(_contextOptions.DatabaseName, null))
                .Returns(_mockMongoDatabase.Object);

            _mockMongoClient.Setup(x => x.StartSession(null, default))
                .Returns(_mockClientSessionHandle.Object);
        }

        private DbSetOperationContextTest CreateContext()
        {
            return new DbSetOperationContextTest(_mockMongoClient.Object, _mockMongoDatabase.Object, _contextOptions);
        }

        [Fact]
        public async Task AddAsync_ReturnSuccess()
        {
            // Arrange
            var context = CreateContext();
            var dbSet = new DbSet<Product>(_mockCollection.Object, context);

            // Act
            await dbSet.AddAsync(new Product());

            // Assert
            _mockCollection.Verify(x => x.InsertOneAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<Product>(), null, default), Times.Once);
            Assert.Single(context.ChangeTracker.Entries);
            Assert.Equal(EntryState.Added, context.ChangeTracker.Entries.First().State);
            Assert.IsType<Product>(context.ChangeTracker.Entries.First().Value);
        }

        [Fact]
        public async Task AddRangeAsync_ReturnSuccess()
        {
            // Arrange
            var context = CreateContext();
            var dbSet = new DbSet<Product>(_mockCollection.Object, context);
            var documents = new List<Product>() { new Product() };

            // Act
            await dbSet.AddRangeAsync(documents);

            // Assert
            _mockCollection.Verify(x => x.InsertManyAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<List<Product>>(), null, default), Times.Once);
            Assert.Equal(documents.Count(), context.ChangeTracker.Entries.Count());
            foreach (var entry in context.ChangeTracker.Entries)
            {
                Assert.Equal(EntryState.Added, entry.State);
                Assert.IsType<Product>(entry.Value);
            }
        }

        [Fact]
        public async Task UpdateAsync_ReturnSuccess()
        {
            // Arrange
            var context = CreateContext();
            var dbSet = new DbSet<Product>(_mockCollection.Object, context);

            // Act
            await dbSet.UpdateAsync(Builders<Product>.Filter.Where(x => x.Name == ""), new Product());

            // Assert
            _mockCollection.Verify(x => x.UpdateOneAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<FilterDefinition<Product>>(), It.IsAny<UpdateDefinition<Product>>(), It.IsAny<UpdateOptions>(), default), Times.Once);
            Assert.Single(context.ChangeTracker.Entries);
            Assert.Equal(EntryState.Modified, context.ChangeTracker.Entries.First().State);
            Assert.IsType<Product>(context.ChangeTracker.Entries.First().Value);
        }

        [Fact]
        public async Task UpdateRangeAsync_ReturnSuccess()
        {
            // Arrange
            var context = CreateContext();
            var dbSet = new DbSet<Product>(_mockCollection.Object, context);

            var bulkOperationModels = new List<BulkOperationModel<Product>>()
            {
                new(Builders<Product>.Filter.Where(x => x.Name == ""), new Product())
            };

            // Act
            await dbSet.UpdateRangeAsync(bulkOperationModels);

            // Assert
            _mockCollection.Verify(x => x.BulkWriteAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<IEnumerable<UpdateOneModel<Product>>>(), It.IsAny<BulkWriteOptions>(), default), Times.Once);
            Assert.Equal(bulkOperationModels.Count(), context.ChangeTracker.Entries.Count());
            foreach (var entry in context.ChangeTracker.Entries)
            {
                Assert.Equal(EntryState.Modified, entry.State);
                Assert.IsType<Product>(entry.Value);
            }
        }

        [Fact]
        public async Task RemoveAsync_ReturnSuccess()
        {
            // Arrange
            var context = CreateContext();
            var dbSet = new DbSet<Product>(_mockCollection.Object, context);

            // Act
            await dbSet.RemoveAsync(Builders<Product>.Filter.Where(x => x.Name == ""), new Product());

            // Assert
            _mockCollection.Verify(x => x.DeleteOneAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<FilterDefinition<Product>>(), null, default), Times.Once);
            Assert.Single(context.ChangeTracker.Entries);
            Assert.Equal(EntryState.Deleted, context.ChangeTracker.Entries.First().State);
            Assert.IsType<Product>(context.ChangeTracker.Entries.First().Value);
        }

        [Fact]
        public async Task RemoveRangeAsync_ReturnSuccess()
        {
            // Arrange
            var context = CreateContext();
            var dbSet = new DbSet<Product>(_mockCollection.Object, context);

            var bulkOperationModels = new List<BulkOperationModel<Product>>()
            {
                new(Builders<Product>.Filter.Where(x => x.Name == ""), new Product())
            };

            // Act
            await dbSet.RemoveRangeAsync(bulkOperationModels);

            // Assert
            _mockCollection.Verify(x => x.BulkWriteAsync(It.IsAny<IClientSessionHandle>(), It.IsAny<IEnumerable<DeleteOneModel<Product>>>(), It.IsAny<BulkWriteOptions>(), default), Times.Once);
            Assert.Equal(bulkOperationModels.Count(), context.ChangeTracker.Entries.Count());
            foreach (var entry in context.ChangeTracker.Entries)
            {
                Assert.Equal(EntryState.Deleted, entry.State);
                Assert.IsType<Product>(entry.Value);
            }
        }
    }
}