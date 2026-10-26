using System;
using Xunit;
using FluentAssertions;
using ButceYonetimi.Entities;

namespace ButceYonetimi.Tests.Expenses
{
    public class ExpenseEntityTests
    {
        [Fact]
        public void Expense_ShouldBeCreatable()
        {
            // Act
            var entity = new Expense();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Expense_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Expense();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Expense_Description_ShouldAcceptValue()
        {
            var entity = new Expense { Description = "Test Value" };
            entity.Description.Should().Be("Test Value");
        }

    }
}
