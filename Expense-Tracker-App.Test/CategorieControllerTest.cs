using Expense_Tracker_App.Models;
using Xunit;
using FluentAssertions;
using Expense_Tracker_App.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Expense_Tracker_App.Test
{
    public class CategorieControllerTest
    {
        private readonly ApplicationDbContext _context;
        private readonly CategorieController _controller;

        public CategorieControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase").Options;
            _context = new ApplicationDbContext(options);
            _controller = new CategorieController(_context);
        }

        #region Index Tests

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfCategories()
        {
            // Arrange 
            _context.Category.Add(new Category { CategoryId = 1 ,Title= "Test" ,Icon="TestIcon" ,Type="TestType"});
            _context.Category.Add(new Category { CategoryId = 2, Title = "Test1", Icon = "TestIcon1", Type = "TestType1" });
            _context.SaveChanges();

            //Act
            var result = await _controller.Index();

            //Assert 
            var ViewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = ViewResult.Model.Should().BeAssignableTo<IEnumerable<Category>>().Subject;
            model.Count().Should().Be(2);
        }

        [Fact]
        public async Task Index_Returns_NotFound_WhenNOCategories()
        {
            //Arrange

            //Act 
            var result = await _controller.Index();

            //Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<Category>>().Subject;
            model.Should().BeNullOrEmpty();
        }

        #endregion

        [Fact]
        public async Task  AddOrEdit_ReturnsViewResult_ForNewCategory()
        {
            //Arrange 
            var category = new Category { CategoryId=0,Title= "Test" ,Icon="Newicon" , Type="NewType"};

            //Act 
            var result = await _controller.AddOrEdit(category);

            // Assert 
           
            result.Should().BeOfType<RedirectToActionResult>();
            _context.Category.Should().HaveCount(1);
            var createadCategory = _context.Category.FirstOrDefault();
            createadCategory.Title.Should().Be(category.Title);
        }

        [Fact]
        public async Task AddOrEdit_ShouldUpdateExistingCategory()
        {
            // Arrange
            var existingCategory = new Category { CategoryId = 1, Title = "Old Title", Icon = "Newicon", Type = "NewType" };
            _context.Category.Add(existingCategory);
            await _context.SaveChangesAsync();

            // Retrieve the existing entity from the context
            var entityToUpdate = await _context.Category.FindAsync(1);

            // Update the properties of the retrieved entity
            entityToUpdate.Title = "New Title";

            // Act
            var result = await _controller.AddOrEdit(entityToUpdate);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            _context.Category.Should().HaveCount(1);
            var updatedEntity = await _context.Category.FindAsync(1);
            updatedEntity.Title.Should().Be("New Title");
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenCategoryNotFound()
        {
            //Arrange

            //Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesCategory_AndRedirectsToIndex()
        {
            //Arrange
            var category = new Category { CategoryId = 1, Title = "DeletedTitle", Icon = "DeletedIcon", Type = "DeletedType" };
            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            //Act 
            var result = await _controller.DeleteConfirmed(1);

            //Assert

            result.Should().BeOfType<RedirectToActionResult>();
            _context.Category.Should().BeEmpty();
        }

        [Fact]
        public void CategoryExists_ShouldReturnTrue_WhenCategoryExists()
        {
            //Arrange
            var category = new Category { CategoryId = 1, Title = "DeletedTitle", Icon = "DeletedIcon", Type = "DeletedType" };
            _context.Category.Add(category);
            _context.SaveChangesAsync();

            //Act 
            var exists = _context.Category.Any(e =>e.CategoryId == 1);

            //Assert
            exists.Should().BeTrue();

        }

        [Fact]
        public void CategoryExists_ShouldReturnFalse_WhenNoCategoryExists()
        {
            //Arrange(no data need to seet)


            //Act 
            var exists = _context.Category.Any(e => e.CategoryId == 1);

            //Assert
            exists.Should().BeFalse();

        }
    }
}