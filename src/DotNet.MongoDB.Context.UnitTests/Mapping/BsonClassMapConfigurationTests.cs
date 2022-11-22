using System;
using System.Linq;
using DotNet.MongoDB.Context.Mapping;
using DotNet.MongoDB.Context.UnitTests.Context.Common;
using MongoDB.Bson.Serialization;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Mapping
{
    public class BsonClassMapConfigurationTests
    {
        public class FakeMapping : BsonClassMapConfiguration<Customer>
        {
            public FakeMapping() : base()
            {
            }

            public FakeMapping(string collectionName) : base(collectionName)
            {
            }

            public override BsonClassMap<Customer> GetConfiguration()
            {
                var map = new BsonClassMap<Customer>();
                map.AutoMap();
                return map;
            }
        }

        public class FakeMappingNullMap : BsonClassMapConfiguration<Customer>
        {
            public FakeMappingNullMap() : base()
            {
            }

            public FakeMappingNullMap(string collectionName) : base(collectionName)
            {
            }

            public override BsonClassMap<Customer> GetConfiguration()
            {
                return null;
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CreateCollectionMap_CollectionNameIsNullOrEmpty_Should_ThrowArgumentNullException(string collectionName)
        {
            // Arrange && Act
            var exception = Assert.Throws<ArgumentNullException>(() => new FakeMapping(collectionName));

            // Assert
            Assert.Equal("Collection name cannot be null or empty. (Parameter 'collectionName')", exception.Message);
        }

        [Fact]
        public void CreateCollectionMap_IsEntity_Should_ReturnTrue()
        {
            // Arrange 
            var collectionName = Guid.NewGuid().ToString();

            // Act
            var map = new FakeMapping(collectionName);

            // Assert
            Assert.True(map.IsEntity);
            Assert.Equal(collectionName, map.CollectionName);
        }

        [Fact]
        public void Apply_BsonClassMapIsNull_DoNothing()
        {
            // Arrange
            var map = new FakeMappingNullMap();

            // Act
            map.Apply();

            // Assert
            Assert.Empty(BsonClassMap.GetRegisteredClassMaps());
        }

        [Fact]
        public void Apply_BsonClassMapExists_DoNothing()
        {
            // Arrange
            var map = new FakeMapping();

            // Act
            map.Apply();
            map.Apply();

            // Assert
            Assert.Single(BsonClassMap.GetRegisteredClassMaps());
        }

        [Fact]
        public void Apply_BsonClassMapNotExists_RegisterClassMap()
        {
            // Arrange
            var map = new FakeMapping();

            // Act
            map.Apply();

            // Assert
            Assert.Single(BsonClassMap.GetRegisteredClassMaps());
        }
    }
}