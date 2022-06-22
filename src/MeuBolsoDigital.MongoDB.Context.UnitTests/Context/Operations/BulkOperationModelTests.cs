using System;
using MeuBolsoDigital.MongoDB.Context.Context.Operations;
using MeuBolsoDigital.MongoDB.Context.UnitTests.Context.Common;
using MongoDB.Driver;
using Xunit;

namespace MeuBolsoDigital.MongoDB.Context.UnitTests.Context.Operations
{
    public class BulkOperationModelTests
    {
        [Fact]
        public void NullFilter_Create_ReturnArgumentNullException()
        {
            // Arrange
            var exceptionMessage = "Filter cannot be null. (Parameter 'filter')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new BulkOperationModel<Product>(null, new Product()));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public void NullDocument_Create_ReturnArgumentNullException()
        {
            // Arrange
            var exceptionMessage = "Document cannot be null. (Parameter 'document')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new BulkOperationModel<Product>(Builders<Product>.Filter.Where(x => x.Name == ""), null));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public void Create_ReturnSuccess()
        {
            // Arrange
            var filter = Builders<Product>.Filter.Where(x => x.Name == "");
            var document = new Product();

            // Act
            var bulkOperationModel = new BulkOperationModel<Product>(filter, document);

            // Assert
            Assert.NotNull(bulkOperationModel.Filter);
            Assert.NotNull(bulkOperationModel.Document);
            Assert.Equal(filter, bulkOperationModel.Filter);
            Assert.Equal(document, bulkOperationModel.Document);
        }
    }
}