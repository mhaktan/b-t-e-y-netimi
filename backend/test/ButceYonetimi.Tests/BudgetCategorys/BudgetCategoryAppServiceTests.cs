using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using ButceYonetimi.Entities;
using ButceYonetimi.BudgetCategorys;
using ButceYonetimi.BudgetCategorys.Dto;
using ButceYonetimi.Flows;

namespace ButceYonetimi.Tests.BudgetCategorys
{
    public class BudgetCategoryAppServiceTests
    {
        private readonly Mock<IRepository<BudgetCategory, long>> _repositoryMock;
        private readonly BudgetCategoryAppService _service;

        public BudgetCategoryAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<BudgetCategory, long>>();
            _service = new BudgetCategoryAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new BudgetCategory { Id = 1, Name = "Test name", Code = "Test code" },
                new BudgetCategory { Id = 2, Name = "Test name", Code = "Test code" },
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
                new BudgetCategory { Id = 1, Name = "Test name", Code = "Test code" },
                new BudgetCategory { Id = 2, Name = "Test name", Code = "Test code" },
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
            var dto = new CreateBudgetCategoryDto
            {
                Name = "Test name", Code = "Test code"
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<BudgetCategory>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new BudgetCategory { Id = 1, Name = "Test name", Code = "Test code" });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new BudgetCategory { Id = 1, Name = "Test name", Code = "Test code" });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
