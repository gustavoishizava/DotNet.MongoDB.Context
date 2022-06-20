using System;
using MeuBolsoDigital.MongoDB.Context.Context.ModelConfiguration;
using MeuBolsoDigital.MongoDB.Context.UnitTests.Context.Common;
using MongoDB.Bson.Serialization;
using Xunit;

namespace MeuBolsoDigital.MongoDB.Context.UnitTests.Context.ModelConfiguration
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
            modelBuilder.AddModelMap(collectionName, new BsonClassMap<ModelTest>());
            modelBuilder.AddModelMap(collectionName, new BsonClassMap<ModelTest>());

            // Assert
            Assert.Single(modelBuilder.ModelMaps);
        }

        [Fact]
        public void ModelMapExists_SameClassType_AddModelMap_DoNothing()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap("products1", new BsonClassMap<ModelTest>());
            modelBuilder.AddModelMap("products2", new BsonClassMap<ModelTest>());

            // Assert
            Assert.Single(modelBuilder.ModelMaps);
        }

        [Fact]
        public void AddModelMap_ReturnSuccess()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap("products1", new BsonClassMap<ModelTest>());
            modelBuilder.AddModelMap("products2", new BsonClassMap<ModelTest2>());

            // Assert
            Assert.Equal(2, modelBuilder.ModelMaps.Count);
        }
    }
}