using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeShare.Application.DTOs;
using RecipeShare.Domain.Interfaces;
using RecipeShare.Domain.Models;
using RecipeShare.Infrastructure.Data;
using RecipeShare.Infrastructure.Services;

namespace RecipeShare.Tests.Services
{
    public class RecipeServiceTests
    {
        private readonly Mock<IRecipeRepository> _mockRepo;
        private readonly AppDbContext _context;
        private readonly RecipeService _service;

        public RecipeServiceTests()
        {
            _mockRepo = new Mock<IRecipeRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            _service = new RecipeService(_mockRepo.Object, _context);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedRecipeDtos()
        {
            // Arrange
            _context.Recipes.Add(new Recipe
            {
                Id = 1,
                Title = "Pasta",
                CookingTime = 30,
                DietaryTags = "Vegetarian",
                Ingredients = "Tomato, Cheese",
                Steps = "Boil pasta"
            });
            await _context.SaveChangesAsync();

            // Act
            List<Recipe> result = await _service.GetAllAsync(null);

            // Assert
            result.Should().HaveCount(1);
            result[0].Title.Should().Be("Pasta");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRecipeDto_WhenFound()
        {
            // Arrange
            Recipe recipe = new Recipe
            {
                Id = 1,
                Title = "Pizza",
                CookingTime = 20,
                Ingredients = "Cheese, Dough",
                Steps = "Bake it",
                DietaryTags = "Vegetarian"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(recipe);

            // Act
            RecipeDto? result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be("Pizza");
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedRecipeDto()
        {
            // Arrange
            RecipeDto dto = new RecipeDto
            {
                Title = "New Dish",
                CookingTime = 15,
                Ingredients = "Egg, Oil",
                Steps = "Fry",
                DietaryTags = "High Protein"
            };
            Recipe savedEntity = new Recipe
            {
                Id = 2,
                Title = dto.Title,
                CookingTime = dto.CookingTime,
                Ingredients = dto.Ingredients,
                Steps = dto.Steps,
                DietaryTags = dto.DietaryTags
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Recipe>())).ReturnsAsync(savedEntity);

            // Act
            RecipeDto result = await _service.CreateAsync(dto);

            // Assert
            result.Id.Should().Be(2);
            result.Title.Should().Be("New Dish");
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateOnce()
        {
            // Arrange
            RecipeDto dto = new RecipeDto
            {
                Id = 3,
                Title = "Updated Dish",
                CookingTime = 45,
                Ingredients = "Stuff",
                Steps = "Cook",
                DietaryTags = "None"
            };

            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Recipe>())).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            _mockRepo.Verify(r => r.UpdateAsync(It.Is<Recipe>(r =>
                r.Id == 3 && r.Title == "Updated Dish")), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteOnce()
        {
            // Arrange
            int id = 5;
            _mockRepo.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(id);

            // Assert
            _mockRepo.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}