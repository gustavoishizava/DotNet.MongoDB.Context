using System;
using MeuBolsoDigital.MongoDB.Context.Configuration;
using Xunit;

namespace MeuBolsoDigital.MongoDB.Context.UnitTests.Configuration
{
    public class MongoDbContextOptionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void InvalidConnectionString_CreateNewMongoDbContextOptions_ReturnArgumentNullExeception(string connectionString)
        {
            // Arrange
            var exceptionMessage = "Connection string cannot be null. (Parameter 'connectionString')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new MongoDbContextOptions(connectionString, "my_database"));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void InvalidDatabaseName_CreateNewMongoDbContextOptions_ReturnArgumentNullExeception(string databaseName)
        {
            // Arrange
            var exceptionMessage = "Database name cannot be null. (Parameter 'databaseName')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new MongoDbContextOptions("connectionString", databaseName));
            Assert.Equal(exceptionMessage, exception.Message);
        }
    }
}