using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models.DTOs;
using ProductAPI.Services;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục với phân trang và lọc
        /// </summary>
        /// <param name="query">Tham số truy vấn</param>
        /// <returns>Danh sách danh mục</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CategoryDto>>>> GetCategories([FromQuery] CategoryQueryDto query)
        {
            try
            {
                var result = await _categoryService.GetAllAsync(query);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: GET /api/categories thành công");
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục");
                return StatusCode(500, ApiResponse<PagedResult<CategoryDto>>.ErrorResult("Lỗi server nội bộ"));
            }
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        /// <param name="id">ID của danh mục</param>
        /// <returns>Thông tin danh mục</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(Guid id)
        {
            try
            {
                var result = await _categoryService.GetByIdAsync(id);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: GET /api/categories/{id} thành công", id);
                    return Ok(result);
                }
                
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin danh mục với ID: {CategoryId}", id);
                return StatusCode(500, ApiResponse<CategoryDto>.ErrorResult("Lỗi server nội bộ"));
            }
        }

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        /// <param name="categoryCreateDto">Thông tin danh mục cần tạo</param>
        /// <returns>Thông tin danh mục đã tạo</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CategoryCreateDto categoryCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<CategoryDto>.ErrorResult("Dữ liệu không hợp lệ"));
                }

                var result = await _categoryService.CreateAsync(categoryCreateDto);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: POST /api/categories thành công");
                    return CreatedAtAction(nameof(GetCategory), new { id = result.Data!.Id }, result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo danh mục");
                return StatusCode(500, ApiResponse<CategoryDto>.ErrorResult("Lỗi server nội bộ"));
            }
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        /// <param name="id">ID của danh mục</param>
        /// <param name="categoryUpdateDto">Thông tin cập nhật</param>
        /// <returns>Thông tin danh mục đã cập nhật</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(Guid id, [FromBody] CategoryUpdateDto categoryUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<CategoryDto>.ErrorResult("Dữ liệu không hợp lệ"));
                }

                var result = await _categoryService.UpdateAsync(id, categoryUpdateDto);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: PUT /api/categories/{id} thành công", id);
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật danh mục với ID: {CategoryId}", id);
                return StatusCode(500, ApiResponse<CategoryDto>.ErrorResult("Lỗi server nội bộ"));
            }
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        /// <param name="id">ID của danh mục</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteAsync(id);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: DELETE /api/categories/{id} thành công", id);
                    return Ok(result);
                }
                
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa danh mục với ID: {CategoryId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Lỗi server nội bộ"));
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục đơn giản (không phân trang)
        /// </summary>
        /// <returns>Danh sách danh mục</returns>
        [HttpGet("simple")]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAllSimpleCategories()
        {
            try
            {
                var result = await _categoryService.GetAllSimpleAsync();
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: GET /api/categories/simple thành công");
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục đơn giản");
                return StatusCode(500, ApiResponse<List<CategoryDto>>.ErrorResult("Lỗi server nội bộ"));
            }
        }
    }
}
