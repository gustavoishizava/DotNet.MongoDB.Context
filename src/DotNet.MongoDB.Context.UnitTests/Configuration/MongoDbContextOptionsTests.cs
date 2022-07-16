using System;
using System.Linq;
using DotNet.MongoDB.Context.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Configuration
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

        [Fact]
        public void CreateNewMongoDbContextOption_ReturnSuccess()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            var connectionString = Guid.NewGuid().ToString();

            // Act
            var options = new MongoDbContextOptions(connectionString, databaseName);

            // Assert
            Assert.Equal(databaseName, options.DatabaseName);
            Assert.Equal(connectionString, options.ConnectionString);
            Assert.Empty(options.Conventions);
            Assert.Empty(options.Serializers);
        }

        [Fact]
        public void AddConvention_ReturnSuccess()
        {
            // Arrange
            var options = new MongoDbContextOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            // Act
            options.AddConvention(new CamelCaseElementNameConvention());

            // Assert
            Assert.Single(options.Conventions);
        }

        [Fact]
        public void AddSerializer_ReturnSuccess()
        {
            // Arrange
            var options = new MongoDbContextOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var bsonSerializer = new GuidSerializer(BsonType.String);

            // Act
            options.AddSerializer(bsonSerializer);

            // Assert
            Assert.Single(options.Serializers);
            Assert.Equal(bsonSerializer, options.Serializers.First().BsonSerializer);
        }
    }
}