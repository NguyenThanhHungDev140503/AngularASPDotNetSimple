using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Data;
using ProductAPI.Extensions;
using ProductAPI.Models.DTOs;
using ProductAPI.Models.Entities;
using ProductAPI.Repositories;
using ProductAPI.Services;
using Xunit;

namespace ProductAPI.Tests
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly IProductRepository _repository;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Setup AutoMapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            // Setup logger mock
            _loggerMock = new Mock<ILogger<ProductService>>();

            // Setup repository and service
            _repository = new ProductRepository(_context);
            _service = new ProductService(_repository, _mapper, _loggerMock.Object);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var category = new Category
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Test Category",
                Description = "Test category description"
            };

            var product = new Product
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Test Product",
                Description = "Test product description",
                StockPrice = 1000000m,
                Price = 1500000m,
                StockQuantity = 10,
                CategoryId = category.Id
            };

            _context.Categories.Add(category);
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var query = new ProductQueryDto { Page = 1, PageSize = 10 };

            // Act
            var result = await _service.GetAllAsync(query);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Items);
            Assert.Equal(1, result.Data.TotalCount);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            var productId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            // Act
            var result = await _service.GetByIdAsync(productId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Test Product", result.Data.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnError()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var result = await _service.GetByIdAsync(invalidId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Không tìm thấy sản phẩm", result.Message);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateProduct()
        {
            // Arrange
            var createDto = new ProductCreateDto
            {
                Name = "New Product",
                Description = "New product description",
                StockPrice = 800000m,
                Price = 1200000m,
                StockQuantity = 5,
                CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            };

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("New Product", result.Data.Name);
        }

        [Fact]
        public async Task CreateAsync_WithPriceLowerThanStockPrice_ShouldReturnError()
        {
            // Arrange
            var createDto = new ProductCreateDto
            {
                Name = "Invalid Product",
                Description = "Invalid product description",
                StockPrice = 1500000m,
                Price = 1000000m, // Lower than stock price
                StockQuantity = 5
            };

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Giá bán không được thấp hơn giá vốn", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateProduct()
        {
            // Arrange
            var productId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var updateDto = new ProductUpdateDto
            {
                Name = "Updated Product",
                Description = "Updated description",
                StockPrice = 1100000m,
                Price = 1600000m,
                StockQuantity = 15
            };

            // Act
            var result = await _service.UpdateAsync(productId, updateDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Updated Product", result.Data.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnError()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var updateDto = new ProductUpdateDto
            {
                Name = "Updated Product",
                Description = "Updated description",
                StockPrice = 1100000m,
                Price = 1600000m,
                StockQuantity = 15
            };

            // Act
            var result = await _service.UpdateAsync(invalidId, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Không tìm thấy sản phẩm", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteProduct()
        {
            // Arrange
            var productId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            // Act
            var result = await _service.DeleteAsync(productId);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnError()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var result = await _service.DeleteAsync(invalidId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Không tìm thấy sản phẩm", result.Message);
        }

        [Fact]
        public async Task GetByCategoryIdAsync_ShouldReturnProductsInCategory()
        {
            // Arrange
            var categoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Act
            var result = await _service.GetByCategoryIdAsync(categoryId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
