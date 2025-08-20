using ProductAPI.Models.DTOs;
using ProductAPI.Models.Entities;

namespace ProductAPI.Repositories
{
    public interface IProductRepository
    {
        Task<PagedResult<Product>> GetAllAsync(ProductQueryDto query);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<List<Product>> GetByCategoryIdAsync(Guid categoryId);
    }
}
