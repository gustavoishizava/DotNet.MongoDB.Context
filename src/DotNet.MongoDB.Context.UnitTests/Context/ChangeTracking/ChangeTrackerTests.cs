using System;
using System.Linq;
using DotNet.MongoDB.Context.Context.ChangeTracking;
using DotNet.MongoDB.Context.UnitTests.Context.Common;
using Xunit;

namespace DotNet.MongoDB.Context.UnitTests.Context.ChangeTracking
{
    public class ChangeTrackerTests
    {
        [Fact]
        public void NullEntry_AddEntry_ReturnArgumentNullException()
        {
            // Arrange
            var changeTracker = new ChangeTracker();
            var exceptionMessage = "Entry cannot be null. (Parameter 'entry')";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => changeTracker.AddEntry(null));
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Theory]
        [InlineData(EntryState.Added)]
        [InlineData(EntryState.Modified)]
        [InlineData(EntryState.Deleted)]
        public void AddEntry_ReturnSuccess(EntryState state)
        {
            // Arrange
            var changeTracker = new ChangeTracker();
            var value = new Product();

            // Act
            changeTracker.AddEntry(new(state, value));

            // Assert
            Assert.Single(changeTracker.Entries);
            Assert.IsType<Product>(changeTracker.Entries.First().Value);
            Assert.Equal(state, changeTracker.Entries.First().State);
        }

        [Fact]
        public void Clear_ReturnEmptyList()
        {
            // Arrange
            var changeTracker = new ChangeTracker();
            var value = new Product();
            changeTracker.AddEntry(new(EntryState.Added, value));
            int oldCount = changeTracker.Entries.Count();

            // Act
            changeTracker.Clear();
            var newCount = changeTracker.Entries.Count();

            // Assert
            Assert.NotEqual(oldCount, newCount);
            Assert.Empty(changeTracker.Entries);
        }
    }
}