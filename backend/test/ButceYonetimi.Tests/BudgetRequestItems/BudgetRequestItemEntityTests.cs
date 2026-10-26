using System;
using Xunit;
using FluentAssertions;
using ButceYonetimi.Entities;

namespace ButceYonetimi.Tests.BudgetRequestItems
{
    public class BudgetRequestItemEntityTests
    {
        [Fact]
        public void BudgetRequestItem_ShouldBeCreatable()
        {
            // Act
            var entity = new BudgetRequestItem();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void BudgetRequestItem_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new BudgetRequestItem();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void BudgetRequestItem_Description_ShouldAcceptValue()
        {
            var entity = new BudgetRequestItem { Description = "Test Value" };
            entity.Description.Should().Be("Test Value");
        }

    }
}
