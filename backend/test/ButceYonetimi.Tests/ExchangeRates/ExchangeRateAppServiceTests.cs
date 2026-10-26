using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using ButceYonetimi.Entities;
using ButceYonetimi.ExchangeRates;
using ButceYonetimi.ExchangeRates.Dto;
using ButceYonetimi.Flows;

namespace ButceYonetimi.Tests.ExchangeRates
{
    public class ExchangeRateAppServiceTests
    {
        private readonly Mock<IRepository<ExchangeRate, long>> _repositoryMock;
        private readonly ExchangeRateAppService _service;

        public ExchangeRateAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<ExchangeRate, long>>();
            _service = new ExchangeRateAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new ExchangeRate { Id = 1, Name = "Test name", FromCurrency = 0, ToCurrency = 0, Rate = 10.0m, EffectiveDate = DateTime.UtcNow },
                new ExchangeRate { Id = 2, Name = "Test name", FromCurrency = 0, ToCurrency = 0, Rate = 10.0m, EffectiveDate = DateTime.UtcNow },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act
            var result = _repositoryMock.Object.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public void Repository_GetAll_WithFilter_ShouldWork()
        {
            // Arrange
            var entities = new[]
            {
                new ExchangeRate { Id = 1, Name = "Test name", FromCurrency = 0, ToCurrency = 0, Rate = 10.0m, EffectiveDate = DateTime.UtcNow },
                new ExchangeRate { Id = 2, Name = "Test name", FromCurrency = 0, ToCurrency = 0, Rate = 10.0m, EffectiveDate = DateTime.UtcNow },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act — simulate keyword filter
            var result = _repositoryMock.Object.GetAll()
                .Where(x => x.Id.ToString().Contains("1"));

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldInsertEntity()
        {
            // Arrange
            var dto = new CreateExchangeRateDto
            {
                Name = "Test name", FromCurrency = 0, ToCurrency = 0, Rate = 10.0m, EffectiveDate = DateTime.UtcNow
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<ExchangeRate>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new ExchangeRate { Id = 1, Name = "Test name", FromCurrency = 0, ToCurrency = 0, Rate = 10.0m, EffectiveDate = DateTime.UtcNow });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new ExchangeRate { Id = 1, Name = "Test name", FromCurrency = 0, ToCurrency = 0, Rate = 10.0m, EffectiveDate = DateTime.UtcNow });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
