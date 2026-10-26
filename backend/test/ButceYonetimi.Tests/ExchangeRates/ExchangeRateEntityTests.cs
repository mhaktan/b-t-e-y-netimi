using System;
using Xunit;
using FluentAssertions;
using ButceYonetimi.Entities;

namespace ButceYonetimi.Tests.ExchangeRates
{
    public class ExchangeRateEntityTests
    {
        [Fact]
        public void ExchangeRate_ShouldBeCreatable()
        {
            // Act
            var entity = new ExchangeRate();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void ExchangeRate_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new ExchangeRate();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void ExchangeRate_Name_ShouldAcceptValue()
        {
            var entity = new ExchangeRate { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
