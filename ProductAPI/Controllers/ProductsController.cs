using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models.DTOs;
using ProductAPI.Services;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm với phân trang và lọc
        /// </summary>
        /// <param name="query">Tham số truy vấn</param>
        /// <returns>Danh sách sản phẩm</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetProducts([FromQuery] ProductQueryDto query)
        {
            try
            {
                var result = await _productService.GetAllAsync(query);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: GET /api/products thành công");
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm");
                return StatusCode(500, ApiResponse<PagedResult<ProductDto>>.ErrorResult("Lỗi server nội bộ"));
            }
        }

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        /// <param name="id">ID của sản phẩm</param>
        /// <returns>Thông tin sản phẩm</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(Guid id)
        {
            try
            {
                var result = await _productService.GetByIdAsync(id);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: GET /api/products/{id} thành công", id);
                    return Ok(result);
                }
                
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sản phẩm với ID: {ProductId}", id);
                return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Lỗi server nội bộ"));
            }
        }

        /// <summary>
        /// Tạo sản phẩm mới
        /// </summary>
        /// <param name="productCreateDto">Thông tin sản phẩm mới</param>
        /// <returns>Sản phẩm đã tạo</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromBody] ProductCreateDto productCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ApiResponse<ProductDto>.ErrorResult("Dữ liệu không hợp lệ", errors));
                }

                var result = await _productService.CreateAsync(productCreateDto);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: POST /api/products thành công");
                    return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.Id }, result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sản phẩm");
                return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Lỗi server nội bộ"));
            }
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        /// <param name="id">ID của sản phẩm</param>
        /// <param name="productUpdateDto">Thông tin cập nhật</param>
        /// <returns>Sản phẩm đã cập nhật</returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(Guid id, [FromBody] ProductUpdateDto productUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ApiResponse<ProductDto>.ErrorResult("Dữ liệu không hợp lệ", errors));
                }

                var result = await _productService.UpdateAsync(id, productUpdateDto);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: PUT /api/products/{id} thành công", id);
                    return Ok(result);
                }
                
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm với ID: {ProductId}", id);
                return StatusCode(500, ApiResponse<ProductDto>.ErrorResult("Lỗi server nội bộ"));
            }
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        /// <param name="id">ID của sản phẩm</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(Guid id)
        {
            try
            {
                var result = await _productService.DeleteAsync(id);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: DELETE /api/products/{id} thành công", id);
                    return Ok(result);
                }
                
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm với ID: {ProductId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Lỗi server nội bộ"));
            }
        }

        /// <summary>
        /// Lấy sản phẩm theo danh mục
        /// </summary>
        /// <param name="categoryId">ID của danh mục</param>
        /// <returns>Danh sách sản phẩm theo danh mục</returns>
        [HttpGet("category/{categoryId:guid}")]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProductsByCategory(Guid categoryId)
        {
            try
            {
                var result = await _productService.GetByCategoryIdAsync(categoryId);
                
                if (result.Success)
                {
                    _logger.LogInformation("Lấy dữ liệu từ: GET /api/products/category/{categoryId} thành công", categoryId);
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sản phẩm theo danh mục với ID: {CategoryId}", categoryId);
                return StatusCode(500, ApiResponse<List<ProductDto>>.ErrorResult("Lỗi server nội bộ"));
            }
        }
    }
}
