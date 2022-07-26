using System.Linq;
using System;
using DotNet.MongoDB.Context.Context.ModelConfiguration;
using DotNet.MongoDB.Context.UnitTests.Context.Common;
using Xunit;
using MongoDB.Bson.Serialization;

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
        public void IsCollection_ModelMapExists_SameCollectionName_AddModelMap_DoNothing()
        {
            // Arrange
            var collectionName = "customers";
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap<Customer>(collectionName, map => { });
            modelBuilder.AddModelMap<Customer>(collectionName, map => { });

            // Assert
            Assert.Single(modelBuilder.ModelMaps);
        }

        [Fact]
        public void IsCollection_ModelMapExists_SameClassType_AddModelMap_DoNothing()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap<Customer>("customers1", map => { });
            modelBuilder.AddModelMap<Customer>("customers2", map => { });

            // Assert
            Assert.Single(modelBuilder.ModelMaps);
        }

        [Fact]
        public void IsNotCollection_ModelMapExists_SameClassType_AddModelMap_DoNothing()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap<Customer>(map => { });
            modelBuilder.AddModelMap<Customer>(map => { });

            // Assert
            Assert.Single(modelBuilder.ModelMaps);
        }

        [Fact]
        public void IsCollection_AddModelMap_ReturnSuccess()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap<Customer>("customer", map => { });
            modelBuilder.AddModelMap<User>("users", map => { });

            // Assert
            var customerMap = modelBuilder.ModelMaps.First(x => x.CollectionName == "customer");
            var userMap = modelBuilder.ModelMaps.First(x => x.CollectionName == "users");

            Assert.Equal(2, modelBuilder.ModelMaps.Count);

            Assert.NotNull(customerMap);
            Assert.NotNull(customerMap.BsonClassMap);
            Assert.True(customerMap.IsCollection);

            Assert.NotNull(userMap);
            Assert.NotNull(userMap.BsonClassMap);
            Assert.True(userMap.IsCollection);
        }

        [Fact]
        public void IsNotCollection_AddModelMap_ReturnSuccess()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();

            // Act
            modelBuilder.AddModelMap<Customer>(map => { });
            modelBuilder.AddModelMap<User>(map => { });

            // Assert
            var customerMap = modelBuilder.ModelMaps.First(x => x.BsonClassMap.ClassType == typeof(Customer));
            var userMap = modelBuilder.ModelMaps.First(x => x.BsonClassMap.ClassType == typeof(User));

            Assert.Equal(2, modelBuilder.ModelMaps.Count);

            Assert.NotNull(customerMap);
            Assert.NotNull(customerMap.BsonClassMap);
            Assert.False(customerMap.IsCollection);

            Assert.NotNull(userMap);
            Assert.NotNull(userMap.BsonClassMap);
            Assert.False(userMap.IsCollection);
        }

        [Fact]
        public void CollectionNoExists_GetCollectionName_ReturnNull()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();
            modelBuilder.AddModelMap<Customer>(map => { });

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