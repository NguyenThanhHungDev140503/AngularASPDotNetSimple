using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models.DTOs;
using ProductAPI.Models.Entities;
using ProductAPI.Repositories;
using Xunit;

namespace ProductAPI.Tests
{
    public class CategoryRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryRepository _repository;

        public CategoryRepositoryTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new CategoryRepository(_context);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Giày thể thao",
                    Description = "Giày dành cho hoạt động thể thao",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Category
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Giày công sở",
                    Description = "Giày lịch sự dành cho môi trường công sở",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Category
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Giày casual",
                    Description = "Giày thường ngày, phong cách thoải mái",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _context.Categories.AddRange(categories);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_WithoutFilters_ReturnsAllCategories()
        {
            // Arrange
            var query = new CategoryQueryDto { Page = 1, PageSize = 10 };

            // Act
            var result = await _repository.GetAllAsync(query);

            // Assert
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
        }

        [Fact]
        public async Task GetAllAsync_WithSearchTerm_ReturnsFilteredCategories()
        {
            // Arrange
            var query = new CategoryQueryDto { SearchTerm = "thể thao", Page = 1, PageSize = 10 };

            // Act
            var result = await _repository.GetAllAsync(query);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.Items.Count);
            Assert.Equal("Giày thể thao", result.Items.First().Name);
        }

        [Fact]
        public async Task GetAllAsync_WithSorting_ReturnsSortedCategories()
        {
            // Arrange
            var query = new CategoryQueryDto { SortBy = "name", SortOrder = "desc", Page = 1, PageSize = 10 };

            // Act
            var result = await _repository.GetAllAsync(query);

            // Assert
            Assert.Equal(3, result.TotalCount);
            Assert.Equal("Giày thể thao", result.Items.First().Name);
            Assert.Equal("Giày casual", result.Items.Last().Name);
        }

        [Fact]
        public async Task GetAllAsync_WithPagination_ReturnsPagedResults()
        {
            // Arrange
            var query = new CategoryQueryDto { Page = 2, PageSize = 2 };

            // Act
            var result = await _repository.GetAllAsync(query);

            // Assert
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(1, result.Items.Count);
            Assert.Equal(2, result.Page);
            Assert.Equal(2, result.PageSize);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCategory()
        {
            // Arrange
            var categoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Act
            var result = await _repository.GetByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal("Giày thể thao", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(categoryId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_WithValidCategory_ReturnsCreatedCategory()
        {
            // Arrange
            var category = new Category
            {
                Name = "Giày boot",
                Description = "Giày boot thời trang"
            };

            // Act
            var result = await _repository.CreateAsync(category);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Giày boot", result.Name);
            Assert.Equal("Giày boot thời trang", result.Description);
        }

        [Fact]
        public async Task UpdateAsync_WithValidCategory_ReturnsUpdatedCategory()
        {
            // Arrange
            var categoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var category = await _repository.GetByIdAsync(categoryId);
            category!.Name = "Giày thể thao cập nhật";
            category.Description = "Mô tả cập nhật";

            // Act
            var result = await _repository.UpdateAsync(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal("Giày thể thao cập nhật", result.Name);
            Assert.Equal("Mô tả cập nhật", result.Description);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var categoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Act
            var result = await _repository.DeleteAsync(categoryId);

            // Assert
            Assert.True(result);
            
            // Verify category is deleted
            var deletedCategory = await _repository.GetByIdAsync(categoryId);
            Assert.Null(deletedCategory);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            var result = await _repository.DeleteAsync(categoryId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var categoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Act
            var result = await _repository.ExistsAsync(categoryId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(categoryId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsByNameAsync_WithExistingName_ReturnsTrue()
        {
            // Act
            var result = await _repository.ExistsByNameAsync("Giày thể thao");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsByNameAsync_WithNonExistingName_ReturnsFalse()
        {
            // Act
            var result = await _repository.ExistsByNameAsync("Giày không tồn tại");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsByNameAsync_WithExcludeId_ReturnsCorrectResult()
        {
            // Arrange
            var excludeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Act
            var result = await _repository.ExistsByNameAsync("Giày thể thao", excludeId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllSimpleAsync_ReturnsAllCategoriesOrderedByName()
        {
            // Act
            var result = await _repository.GetAllSimpleAsync();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Giày casual", result.First().Name);
            Assert.Equal("Giày thể thao", result.Last().Name);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
