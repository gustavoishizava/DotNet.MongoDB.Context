using System;
using DotNet.MongoDB.Context.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Configuration
{
    public class SerializerTests
    {
        [Fact]
        public void Create_BsonSerializerNull_ReturnArgumentNullException()
        {
            // Arrange
            var exceptionMessage = "BsonSerializer cannot be null. (Parameter 'bsonSerializer')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new Serializer(null));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public void Create_ReturnSuccess()
        {
            // Arrange
            var bsonSerializer = new GuidSerializer(BsonType.String);

            // Act
            var serializer = new Serializer(bsonSerializer);

            // Assert
            Assert.Equal(bsonSerializer, serializer.BsonSerializer);
            Assert.False(serializer.Registered);
        }

        [Fact]
        public void Register_ReturnRegisteredTrue()
        {
            // Arrange
            var bsonSerializer = new GuidSerializer(BsonType.String);
            var serializer = new Serializer(bsonSerializer);

            // Act
            serializer.Register();

            // Assert
            Assert.True(serializer.Registered);
        }
    }
}