using ProductAPI.Models.DTOs;

namespace ProductAPI.Services
{
    public interface ICategoryService
    {
        Task<ApiResponse<PagedResult<CategoryDto>>> GetAllAsync(CategoryQueryDto query);
        Task<ApiResponse<CategoryDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<CategoryDto>> CreateAsync(CategoryCreateDto categoryCreateDto);
        Task<ApiResponse<CategoryDto>> UpdateAsync(Guid id, CategoryUpdateDto categoryUpdateDto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
        Task<ApiResponse<List<CategoryDto>>> GetAllSimpleAsync();
    }
}
