using System;
using Xunit;
using FluentAssertions;
using ButceYonetimi.Entities;

namespace ButceYonetimi.Tests.BudgetPeriods
{
    public class BudgetPeriodEntityTests
    {
        [Fact]
        public void BudgetPeriod_ShouldBeCreatable()
        {
            // Act
            var entity = new BudgetPeriod();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void BudgetPeriod_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new BudgetPeriod();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsActive.Should().Be(false);
        }

        [Fact]
        public void BudgetPeriod_Name_ShouldAcceptValue()
        {
            var entity = new BudgetPeriod { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
