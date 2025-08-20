using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models.DTOs;
using ProductAPI.Models.Entities;

namespace ProductAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Product>> GetAllAsync(ProductQueryDto query)
        {
            var queryable = _context.Products.Include(p => p.Category).AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                queryable = queryable.Where(p => p.Name.Contains(query.SearchTerm) || 
                                                (p.Description != null && p.Description.Contains(query.SearchTerm)));
            }

            if (query.CategoryId.HasValue)
            {
                queryable = queryable.Where(p => p.CategoryId == query.CategoryId);
            }

            if (query.MinPrice.HasValue)
            {
                queryable = queryable.Where(p => p.Price >= query.MinPrice);
            }

            if (query.MaxPrice.HasValue)
            {
                queryable = queryable.Where(p => p.Price <= query.MaxPrice);
            }

            // Apply sorting
            queryable = query.SortBy?.ToLower() switch
            {
                "name" => query.SortOrder?.ToLower() == "desc" 
                    ? queryable.OrderByDescending(p => p.Name)
                    : queryable.OrderBy(p => p.Name),
                "price" => query.SortOrder?.ToLower() == "desc"
                    ? queryable.OrderByDescending(p => p.Price)
                    : queryable.OrderBy(p => p.Price),
                "createdat" => query.SortOrder?.ToLower() == "desc"
                    ? queryable.OrderByDescending(p => p.CreatedAt)
                    : queryable.OrderBy(p => p.CreatedAt),
                _ => queryable.OrderBy(p => p.Name)
            };

            var totalCount = await queryable.CountAsync();

            // Apply pagination
            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(product.Id) ?? product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(product.Id) ?? product;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
