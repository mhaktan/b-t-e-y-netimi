using System;
using Xunit;
using FluentAssertions;
using ButceYonetimi.Entities;

namespace ButceYonetimi.Tests.BudgetCategorys
{
    public class BudgetCategoryEntityTests
    {
        [Fact]
        public void BudgetCategory_ShouldBeCreatable()
        {
            // Act
            var entity = new BudgetCategory();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void BudgetCategory_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new BudgetCategory();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void BudgetCategory_Name_ShouldAcceptValue()
        {
            var entity = new BudgetCategory { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

        [Fact]
        public void BudgetCategory_Code_ShouldAcceptValue()
        {
            var entity = new BudgetCategory { Code = "Test Value" };
            entity.Code.Should().Be("Test Value");
        }

    }
}
