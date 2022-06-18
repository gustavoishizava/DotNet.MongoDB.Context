using System;
using System.Collections.Generic;
using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.Context;
using Xunit;

namespace MeuBolsoDigital.MongoDB.Context.UnitTests.Context
{
    public class DbContextTests
    {
        private class TestDbContext : DbContext
        {
            public TestDbContext(MongoDbContextOptions options) : base(options)
            {
            }

            public override Dictionary<Type, string> ConfigureCollections()
            {
                return new Dictionary<Type, string>();
            }
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
    }
}