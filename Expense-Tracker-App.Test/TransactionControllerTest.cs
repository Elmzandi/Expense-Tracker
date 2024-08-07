using Expense_Tracker_App.Models;
using Xunit;
using FluentAssertions;
using Expense_Tracker_App.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Expense_Tracker_App.Test
{
    public class TransactionControllerTest
    {
        private readonly ApplicationDbContext _context;
        private readonly TransactionController _controller;

        public TransactionControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase").Options;
            _context = new ApplicationDbContext(options);
            _controller = new TransactionController(_context);
        }

        #region Index Tests

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfTransaction()
        {
            // Arrange 
            _context.Category.Add(new Category { CategoryId = 1, Title = "Test", Icon = "TestIcon", Type = "TestType" });
            _context.Transaction.Add(new Transaction { Id = 1, CategoryId = 1 , Amount = 100, Note = "TestNote", Date = DateTime.Now });
            _context.Transaction.Add(new Transaction { Id = 2, CategoryId = 1, Amount = 100, Note = "TestNote", Date = DateTime.Now });
            await _context.SaveChangesAsync();

            //Act
            var result = await _controller.Index();

            //Assert 
            var ViewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = ViewResult.Model.Should().BeAssignableTo<IEnumerable<Transaction>>().Subject;
            model.Count().Should().Be(2);
            model.First().Category.Should().NotBeNull();
        }

        [Fact]
        public async Task Index_Returns_NotFound_WhenNOTransaction()
        {
            //Arrange

            //Act 
            var result = await _controller.Index();

            //Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<Transaction>>().Subject;
            model.Should().BeNullOrEmpty();
        }

        #endregion

        [Fact]
        public async Task  AddOrEdit_ReturnsViewResult_ForNewTransaction()
        {
            //Arrange 
            var Category = new Category { CategoryId = 1, Title = "Test", Icon = "TestIcon", Type = "TestType" };
            var Transaction = new Transaction { Id = 1, CategoryId = 1, Amount = 100, Note = "TestNote", Date = DateTime.Now };

            //Act 
            var result = await _controller.AddOrEdit(Transaction);

            // Assert 
           
            result.Should().BeOfType<RedirectToActionResult>();
            _context.Transaction.Should().HaveCount(1);
            var createadTransaction = _context.Transaction.FirstOrDefault();
            createadTransaction.Amount.Should().Be(Transaction.Amount);
        }

        [Fact]
        public async Task AddOrEdit_ShouldUpdateExistingTransaction()
        {
            // Arrange
            var existingTransaction = new Transaction { Id = 1, CategoryId = 1, Amount = 100, Note = "TestNote", Date = DateTime.Now };
            _context.Transaction.Add(existingTransaction);
            await _context.SaveChangesAsync();

            existingTransaction.Amount = 200;

            // Act
            var result = await _controller.AddOrEdit(existingTransaction);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            _context.Transaction.Should().HaveCount(1);
            var updatedEntity = _context.Transaction.FirstOrDefault();
            updatedEntity.Amount.Should().Be(200);
        }


        [Fact]
        public async Task DeleteConfirmed_DeletesTransaction_AndRedirectsToIndex()
        {
            //Arrange

            var Transaction = new Transaction { Id = 1, CategoryId = 1, Amount = 100, Note = "TestNote", Date = DateTime.Now };

            //Act 
            var result = await _controller.DeleteConfirmed(1);

            //Assert

            result.Should().BeOfType<RedirectToActionResult>();
            _context.Transaction.Should().BeEmpty();
        }

        [Fact]
        public void TransactionExists_ShouldReturnTrue_WhenTransactionExists()
        {
            //Arrange
            var Transaction = new Transaction {Id = 1, CategoryId = 1, Amount = 100, Note = "TestNote", Date = DateTime.Now };
            _context.Transaction.Add(Transaction);
            _context.SaveChangesAsync();

            //Act 
            var exists = _context.Transaction.Any(e =>e.Id == 1);

            //Assert
            exists.Should().BeTrue();

        }

        [Fact]
        public void TransactionExists_ShouldReturnFalse_WhenNoTransactionExists()
        {
            //Arrange(no data need to seet)


            //Act 
            var exists = _context.Transaction.Any(e => e.Id == 1);

            //Assert
            exists.Should().BeFalse();

        }
    }
}