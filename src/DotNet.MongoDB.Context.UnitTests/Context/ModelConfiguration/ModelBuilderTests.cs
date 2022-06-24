using System;
using DotNet.MongoDB.Context.Context.ModelConfiguration;
using DotNet.MongoDB.Context.UnitTests.Context.Common;
using MongoDB.Bson.Serialization;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Context.ModelConfiguration
{
    public class ModelBuilderTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CollectionNameInvalid_AddModelMap_ReturnArgumentNullException(string collectionName)
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => modelBuilder.AddModelMap(collectionName, null));
            Assert.Equal("Collection name cannot be null. (Parameter 'collectionName')", exception.Message);
        }

        [Fact]
        public void BsonClassMapNull_AddModelMap_ReturnArgumentNullException()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => modelBuilder.AddModelMap("collectionName", null));
            Assert.Equal("BsonClassMap cannot be null. (Parameter 'bsonClassMap')", exception.Message);
        }

        [Fact]
        public void ModelMapExists_SameCollectionName_AddModelMap_DoNothing()
        {
            // Arrange
            var collectionName = "products";
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap(collectionName, new BsonClassMap<Customer>());
            modelBuilder.AddModelMap(collectionName, new BsonClassMap<Customer>());

            // Assert
            Assert.Single(modelBuilder.ModelMaps);
        }

        [Fact]
        public void ModelMapExists_SameClassType_AddModelMap_DoNothing()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap("products1", new BsonClassMap<Customer>());
            modelBuilder.AddModelMap("products2", new BsonClassMap<Customer>());

            // Assert
            Assert.Single(modelBuilder.ModelMaps);
        }

        [Fact]
        public void AddModelMap_ReturnSuccess()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap("products1", new BsonClassMap<Customer>());
            modelBuilder.AddModelMap("products2", new BsonClassMap<User>());

            // Assert
            Assert.Equal(2, modelBuilder.ModelMaps.Count);
        }

        [Fact]
        public void CollectionNoExists_GetCollectionName_ReturnNull()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            var result = modelBuilder.GetCollectionName(typeof(Customer));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CollectionExists_GetCollectionName_ReturnName()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();
            var collectionName = "products";
            modelBuilder.AddModelMap(collectionName, new BsonClassMap<Customer>());

            // Act
            var result = modelBuilder.GetCollectionName(typeof(Customer));

            // Assert
            Assert.Equal(collectionName, result);
        }
    }
}