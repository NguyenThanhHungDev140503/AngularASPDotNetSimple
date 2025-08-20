using AutoMapper;
using ProductAPI.Models.DTOs;
using ProductAPI.Models.Entities;
using ProductAPI.Repositories;

namespace ProductAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<PagedResult<CategoryDto>>> GetAllAsync(CategoryQueryDto query)
        {
            try
            {
                var result = await _categoryRepository.GetAllAsync(query);
                var categoryDtos = _mapper.Map<List<CategoryDto>>(result.Items);

                var pagedResult = new PagedResult<CategoryDto>
                {
                    Items = categoryDtos,
                    TotalCount = result.TotalCount,
                    Page = result.Page,
                    PageSize = result.PageSize
                };

                return ApiResponse<PagedResult<CategoryDto>>.SuccessResult(pagedResult, "Lấy danh sách danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục");
                return ApiResponse<PagedResult<CategoryDto>>.ErrorResult("Có lỗi xảy ra khi lấy danh sách danh mục");
            }
        }

        public async Task<ApiResponse<CategoryDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return ApiResponse<CategoryDto>.ErrorResult("Không tìm thấy danh mục");
                }

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return ApiResponse<CategoryDto>.SuccessResult(categoryDto, "Lấy thông tin danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin danh mục với ID: {CategoryId}", id);
                return ApiResponse<CategoryDto>.ErrorResult("Có lỗi xảy ra khi lấy thông tin danh mục");
            }
        }

        public async Task<ApiResponse<CategoryDto>> CreateAsync(CategoryCreateDto categoryCreateDto)
        {
            try
            {
                // Validate business rules
                var existsByName = await _categoryRepository.ExistsByNameAsync(categoryCreateDto.Name);
                if (existsByName)
                {
                    return ApiResponse<CategoryDto>.ErrorResult("Tên danh mục đã tồn tại");
                }

                var category = _mapper.Map<Category>(categoryCreateDto);
                var createdCategory = await _categoryRepository.CreateAsync(category);
                var categoryDto = _mapper.Map<CategoryDto>(createdCategory);

                return ApiResponse<CategoryDto>.SuccessResult(categoryDto, "Tạo danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo danh mục");
                return ApiResponse<CategoryDto>.ErrorResult("Có lỗi xảy ra khi tạo danh mục");
            }
        }

        public async Task<ApiResponse<CategoryDto>> UpdateAsync(Guid id, CategoryUpdateDto categoryUpdateDto)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return ApiResponse<CategoryDto>.ErrorResult("Không tìm thấy danh mục");
                }

                // Validate business rules
                var existsByName = await _categoryRepository.ExistsByNameAsync(categoryUpdateDto.Name, id);
                if (existsByName)
                {
                    return ApiResponse<CategoryDto>.ErrorResult("Tên danh mục đã tồn tại");
                }

                // Update properties
                existingCategory.Name = categoryUpdateDto.Name;
                existingCategory.Description = categoryUpdateDto.Description;

                var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
                var categoryDto = _mapper.Map<CategoryDto>(updatedCategory);

                return ApiResponse<CategoryDto>.SuccessResult(categoryDto, "Cập nhật danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật danh mục với ID: {CategoryId}", id);
                return ApiResponse<CategoryDto>.ErrorResult("Có lỗi xảy ra khi cập nhật danh mục");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var exists = await _categoryRepository.ExistsAsync(id);
                if (!exists)
                {
                    return ApiResponse<bool>.ErrorResult("Không tìm thấy danh mục");
                }

                var result = await _categoryRepository.DeleteAsync(id);
                if (result)
                {
                    return ApiResponse<bool>.SuccessResult(true, "Xóa danh mục thành công");
                }

                return ApiResponse<bool>.ErrorResult("Không thể xóa danh mục");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa danh mục với ID: {CategoryId}", id);
                return ApiResponse<bool>.ErrorResult("Có lỗi xảy ra khi xóa danh mục");
            }
        }

        public async Task<ApiResponse<List<CategoryDto>>> GetAllSimpleAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllSimpleAsync();
                var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

                return ApiResponse<List<CategoryDto>>.SuccessResult(categoryDtos, "Lấy danh sách danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục đơn giản");
                return ApiResponse<List<CategoryDto>>.ErrorResult("Có lỗi xảy ra khi lấy danh sách danh mục");
            }
        }
    }
}
