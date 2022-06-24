using System;
using DotNet.MongoDB.Context.Context.ChangeTracking;
using DotNet.MongoDB.Context.UnitTests.Context.Common;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Context.ChangeTracking
{
    public class EntryTests
    {
        [Fact]
        public void ValueNull_CreateEntry_ReturnArgumentNullException()
        {
            // Arrange
            var state = EntryState.Added;
            object value = null;
            var exceptionMessage = "Value cannot be null. (Parameter 'value')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new Entry(state, value));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Theory]
        [InlineData(EntryState.Added)]
        [InlineData(EntryState.Modified)]
        [InlineData(EntryState.Deleted)]
        public void CreateEntry_ReturnSuccess(EntryState state)
        {
            // Arrange
            var value = new Product();

            // Act
            var entry = new Entry(state, value);

            // Assert
            Assert.Equal(state, entry.State);
            Assert.NotNull(entry.Value);
            Assert.IsType<Product>(entry.Value);
        }
    }
}