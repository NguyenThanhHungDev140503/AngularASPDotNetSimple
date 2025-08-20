using ProductAPI.Models.DTOs;

namespace ProductAPI.Services
{
    public interface IProductService
    {
        Task<ApiResponse<PagedResult<ProductDto>>> GetAllAsync(ProductQueryDto query);
        Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<ProductDto>> CreateAsync(ProductCreateDto productCreateDto);
        Task<ApiResponse<ProductDto>> UpdateAsync(Guid id, ProductUpdateDto productUpdateDto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
        Task<ApiResponse<List<ProductDto>>> GetByCategoryIdAsync(Guid categoryId);
    }
}
