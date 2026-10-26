using System;
using Xunit;
using FluentAssertions;
using ButceYonetimi.Entities;

namespace ButceYonetimi.Tests.BudgetRequests
{
    public class BudgetRequestEntityTests
    {
        [Fact]
        public void BudgetRequest_ShouldBeCreatable()
        {
            // Act
            var entity = new BudgetRequest();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void BudgetRequest_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new BudgetRequest();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void BudgetRequest_Title_ShouldAcceptValue()
        {
            var entity = new BudgetRequest { Title = "Test Value" };
            entity.Title.Should().Be("Test Value");
        }

        [Fact]
        public void BudgetRequest_Justification_ShouldAcceptValue()
        {
            var entity = new BudgetRequest { Justification = "Test Value" };
            entity.Justification.Should().Be("Test Value");
        }

    }
}
