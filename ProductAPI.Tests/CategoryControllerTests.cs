using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Controllers;
using ProductAPI.Models.DTOs;
using ProductAPI.Services;
using Xunit;

namespace ProductAPI.Tests
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly Mock<ILogger<CategoriesController>> _loggerMock;
        private readonly CategoriesController _controller;

        public CategoryControllerTests()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            _loggerMock = new Mock<ILogger<CategoriesController>>();
            _controller = new CategoriesController(_categoryServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetCategories_WithValidQuery_ReturnsOkResult()
        {
            // Arrange
            var query = new CategoryQueryDto { Page = 1, PageSize = 10 };
            var categories = new List<CategoryDto>
            {
                new CategoryDto { Id = Guid.NewGuid(), Name = "Test Category 1" },
                new CategoryDto { Id = Guid.NewGuid(), Name = "Test Category 2" }
            };
            var pagedResult = new PagedResult<CategoryDto>
            {
                Items = categories,
                TotalCount = 2,
                Page = 1,
                PageSize = 10
            };
            var serviceResponse = ApiResponse<PagedResult<CategoryDto>>.SuccessResult(pagedResult, "Success");

            _categoryServiceMock.Setup(s => s.GetAllAsync(query))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetCategories(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<PagedResult<CategoryDto>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(2, response.Data!.Items.Count);
        }

        [Fact]
        public async Task GetCategories_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            var query = new CategoryQueryDto { Page = 1, PageSize = 10 };
            var serviceResponse = ApiResponse<PagedResult<CategoryDto>>.ErrorResult("Service error");

            _categoryServiceMock.Setup(s => s.GetAllAsync(query))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetCategories(query);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<PagedResult<CategoryDto>>>(badRequestResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task GetCategory_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new CategoryDto { Id = categoryId, Name = "Test Category" };
            var serviceResponse = ApiResponse<CategoryDto>.SuccessResult(category, "Success");

            _categoryServiceMock.Setup(s => s.GetByIdAsync(categoryId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetCategory(categoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CategoryDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(categoryId, response.Data!.Id);
        }

        [Fact]
        public async Task GetCategory_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var serviceResponse = ApiResponse<CategoryDto>.ErrorResult("Category not found");

            _categoryServiceMock.Setup(s => s.GetByIdAsync(categoryId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetCategory(categoryId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CategoryDto>>(notFoundResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task CreateCategory_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var createDto = new CategoryCreateDto { Name = "New Category", Description = "Description" };
            var createdCategory = new CategoryDto { Id = Guid.NewGuid(), Name = "New Category", Description = "Description" };
            var serviceResponse = ApiResponse<CategoryDto>.SuccessResult(createdCategory, "Created");

            _categoryServiceMock.Setup(s => s.CreateAsync(createDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.CreateCategory(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CategoryDto>>(createdResult.Value);
            Assert.True(response.Success);
            Assert.Equal("New Category", response.Data!.Name);
        }

        [Fact]
        public async Task CreateCategory_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CategoryCreateDto { Name = "Duplicate Category" };
            var serviceResponse = ApiResponse<CategoryDto>.ErrorResult("Category name already exists");

            _categoryServiceMock.Setup(s => s.CreateAsync(createDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.CreateCategory(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CategoryDto>>(badRequestResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdateCategory_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var updateDto = new CategoryUpdateDto { Name = "Updated Category", Description = "Updated Description" };
            var updatedCategory = new CategoryDto { Id = categoryId, Name = "Updated Category", Description = "Updated Description" };
            var serviceResponse = ApiResponse<CategoryDto>.SuccessResult(updatedCategory, "Updated");

            _categoryServiceMock.Setup(s => s.UpdateAsync(categoryId, updateDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.UpdateCategory(categoryId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CategoryDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Updated Category", response.Data!.Name);
        }

        [Fact]
        public async Task UpdateCategory_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var updateDto = new CategoryUpdateDto { Name = "Duplicate Name" };
            var serviceResponse = ApiResponse<CategoryDto>.ErrorResult("Category name already exists");

            _categoryServiceMock.Setup(s => s.UpdateAsync(categoryId, updateDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.UpdateCategory(categoryId, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CategoryDto>>(badRequestResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task DeleteCategory_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var serviceResponse = ApiResponse<bool>.SuccessResult(true, "Deleted");

            _categoryServiceMock.Setup(s => s.DeleteAsync(categoryId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.DeleteCategory(categoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
            Assert.True(response.Success);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task DeleteCategory_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var serviceResponse = ApiResponse<bool>.ErrorResult("Category not found");

            _categoryServiceMock.Setup(s => s.DeleteAsync(categoryId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.DeleteCategory(categoryId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task GetAllSimpleCategories_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var categories = new List<CategoryDto>
            {
                new CategoryDto { Id = Guid.NewGuid(), Name = "Category 1" },
                new CategoryDto { Id = Guid.NewGuid(), Name = "Category 2" }
            };
            var serviceResponse = ApiResponse<List<CategoryDto>>.SuccessResult(categories, "Success");

            _categoryServiceMock.Setup(s => s.GetAllSimpleAsync())
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetAllSimpleCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<CategoryDto>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(2, response.Data!.Count);
        }

        [Fact]
        public async Task GetAllSimpleCategories_WithServiceError_ReturnsBadRequest()
        {
            // Arrange
            var serviceResponse = ApiResponse<List<CategoryDto>>.ErrorResult("Service error");

            _categoryServiceMock.Setup(s => s.GetAllSimpleAsync())
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetAllSimpleCategories();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<List<CategoryDto>>>(badRequestResult.Value);
            Assert.False(response.Success);
        }
    }
}
