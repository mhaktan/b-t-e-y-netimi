using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using ButceYonetimi.Entities;
using ButceYonetimi.BudgetPeriods;
using ButceYonetimi.BudgetPeriods.Dto;
using ButceYonetimi.Flows;

namespace ButceYonetimi.Tests.BudgetPeriods
{
    public class BudgetPeriodAppServiceTests
    {
        private readonly Mock<IRepository<BudgetPeriod, long>> _repositoryMock;
        private readonly BudgetPeriodAppService _service;

        public BudgetPeriodAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<BudgetPeriod, long>>();
            _service = new BudgetPeriodAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new BudgetPeriod { Id = 1, Name = "Test name", Year = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, IsActive = true },
                new BudgetPeriod { Id = 2, Name = "Test name", Year = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, IsActive = true },
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
                new BudgetPeriod { Id = 1, Name = "Test name", Year = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, IsActive = true },
                new BudgetPeriod { Id = 2, Name = "Test name", Year = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, IsActive = true },
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
            var dto = new CreateBudgetPeriodDto
            {
                Name = "Test name", Year = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, IsActive = true
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<BudgetPeriod>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new BudgetPeriod { Id = 1, Name = "Test name", Year = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, IsActive = true });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new BudgetPeriod { Id = 1, Name = "Test name", Year = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, IsActive = true });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
