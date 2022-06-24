using System.Linq;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using DotNet.MongoDB.Context.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private class ExtensionsContextTest : DbContext
        {
            public ExtensionsContextTest(MongoDbContextOptions options) : base(options)
            {
            }
        }

        private Mock<IMongoClient> _mockMongoClient;
        private Mock<IMongoDatabase> _mockMongoDatabase;
        private Mock<IClientSessionHandle> _mockClientSessionHandle;
        private MongoDbContextOptions _contextOptions;

        public ServiceCollectionExtensionsTests()
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

        [Fact]
        public void AddMongoDbContext_ReturnSuccess()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddMongoDbContext<ExtensionsContextTest>(options =>
            {
                options.ConfigureConnection(_contextOptions.ConnectionString, _contextOptions.DatabaseName);
            });

            var context = services.FirstOrDefault(x => x.ImplementationType == typeof(ExtensionsContextTest));
            var options = services.FirstOrDefault(x => x.ServiceType == typeof(MongoDbContextOptions));

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(options);
            Assert.Equal(ServiceLifetime.Scoped, context.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, options.Lifetime);
        }
    }
}