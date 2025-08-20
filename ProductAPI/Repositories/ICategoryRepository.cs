using ProductAPI.Models.DTOs;
using ProductAPI.Models.Entities;

namespace ProductAPI.Repositories
{
    public interface ICategoryRepository
    {
        Task<PagedResult<Category>> GetAllAsync(CategoryQueryDto query);
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
        Task<List<Category>> GetAllSimpleAsync();
    }
}
