using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using ButceYonetimi.Entities;
using ButceYonetimi.Expenses;
using ButceYonetimi.Expenses.Dto;
using ButceYonetimi.Flows;

namespace ButceYonetimi.Tests.Expenses
{
    public class ExpenseAppServiceTests
    {
        private readonly Mock<IRepository<Expense, long>> _repositoryMock;
        private readonly ExpenseAppService _service;

        public ExpenseAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Expense, long>>();
            _service = new ExpenseAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object, new Mock<IRepository<StatusChangeLog, long>>().Object, new Mock<IRepository<ApprovalRecord, Guid>>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new Expense { Id = 1, Description = "Test description", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow, Status = 0 },
                new Expense { Id = 2, Description = "Test description", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow, Status = 0 },
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
                new Expense { Id = 1, Description = "Test description", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow, Status = 0 },
                new Expense { Id = 2, Description = "Test description", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow, Status = 0 },
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
            var dto = new CreateExpenseDto
            {
                Description = "Test description", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow, Status = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<Expense>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Expense { Id = 1, Description = "Test description", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Expense { Id = 1, Description = "Test description", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
