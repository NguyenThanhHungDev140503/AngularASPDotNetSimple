using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models.DTOs;
using ProductAPI.Models.Entities;

namespace ProductAPI.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Category>> GetAllAsync(CategoryQueryDto query)
        {
            var queryable = _context.Categories.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                queryable = queryable.Where(c => c.Name.Contains(query.SearchTerm) || 
                                                (c.Description != null && c.Description.Contains(query.SearchTerm)));
            }

            // Apply sorting
            queryable = query.SortBy?.ToLower() switch
            {
                "name" => query.SortOrder?.ToLower() == "desc" 
                    ? queryable.OrderByDescending(c => c.Name)
                    : queryable.OrderBy(c => c.Name),
                "createdat" => query.SortOrder?.ToLower() == "desc"
                    ? queryable.OrderByDescending(c => c.CreatedAt)
                    : queryable.OrderBy(c => c.CreatedAt),
                "updatedat" => query.SortOrder?.ToLower() == "desc"
                    ? queryable.OrderByDescending(c => c.UpdatedAt)
                    : queryable.OrderBy(c => c.UpdatedAt),
                _ => queryable.OrderBy(c => c.Name)
            };

            var totalCount = await queryable.CountAsync();

            // Apply pagination
            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Category>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(category.Id) ?? category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(category.Id) ?? category;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.Name.ToLower() == name.ToLower());
            
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<List<Category>> GetAllSimpleAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
