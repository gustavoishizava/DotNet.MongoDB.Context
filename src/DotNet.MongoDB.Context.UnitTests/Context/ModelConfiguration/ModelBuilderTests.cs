using System;
using DotNet.MongoDB.Context.Context.ModelConfiguration;
using DotNet.MongoDB.Context.UnitTests.Context.Common;
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
            var exception = Assert.Throws<ArgumentNullException>(() => modelBuilder.AddModelMap<Customer>(collectionName, null));
            Assert.Equal("Collection name cannot be null. (Parameter 'collectionName')", exception.Message);
        }

        [Fact]
        public void ModelMapExists_SameCollectionName_AddModelMap_DoNothing()
        {
            // Arrange
            var collectionName = "products";
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap<Customer>(collectionName, map => { });
            modelBuilder.AddModelMap<Customer>(collectionName, map => { });

            // Assert
            Assert.Single(modelBuilder.ModelMaps);
        }

        [Fact]
        public void ModelMapExists_SameClassType_AddModelMap_DoNothing()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap<Customer>("products1", map => { });
            modelBuilder.AddModelMap<Customer>("products2", map => { });

            // Assert
            Assert.Single(modelBuilder.ModelMaps);
        }

        [Fact]
        public void AddModelMap_ReturnSuccess()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap<Customer>("products1", map => { });
            modelBuilder.AddModelMap<User>("products2", map => { });

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
            modelBuilder.AddModelMap<Customer>(collectionName, null);

            // Act
            var result = modelBuilder.GetCollectionName(typeof(Customer));

            // Assert
            Assert.Equal(collectionName, result);
        }
    }
}