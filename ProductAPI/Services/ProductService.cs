using AutoMapper;
using ProductAPI.Models.DTOs;
using ProductAPI.Models.Entities;
using ProductAPI.Repositories;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<PagedResult<ProductDto>>> GetAllAsync(ProductQueryDto query)
        {
            try
            {
                var result = await _productRepository.GetAllAsync(query);
                var productDtos = _mapper.Map<List<ProductDto>>(result.Items);

                var pagedResult = new PagedResult<ProductDto>
                {
                    Items = productDtos,
                    TotalCount = result.TotalCount,
                    Page = result.Page,
                    PageSize = result.PageSize
                };

                return ApiResponse<PagedResult<ProductDto>>.SuccessResult(pagedResult, "Lấy danh sách sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm");
                return ApiResponse<PagedResult<ProductDto>>.ErrorResult("Có lỗi xảy ra khi lấy danh sách sản phẩm");
            }
        }

        public async Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return ApiResponse<ProductDto>.ErrorResult("Không tìm thấy sản phẩm");
                }

                var productDto = _mapper.Map<ProductDto>(product);
                return ApiResponse<ProductDto>.SuccessResult(productDto, "Lấy thông tin sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin sản phẩm với ID: {ProductId}", id);
                return ApiResponse<ProductDto>.ErrorResult("Có lỗi xảy ra khi lấy thông tin sản phẩm");
            }
        }

        public async Task<ApiResponse<ProductDto>> CreateAsync(ProductCreateDto productCreateDto)
        {
            try
            {
                // Validate business rules
                if (productCreateDto.Price < productCreateDto.StockPrice)
                {
                    return ApiResponse<ProductDto>.ErrorResult("Giá bán không được thấp hơn giá vốn");
                }

                var product = _mapper.Map<Product>(productCreateDto);
                var createdProduct = await _productRepository.CreateAsync(product);
                var productDto = _mapper.Map<ProductDto>(createdProduct);

                return ApiResponse<ProductDto>.SuccessResult(productDto, "Tạo sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sản phẩm");
                return ApiResponse<ProductDto>.ErrorResult("Có lỗi xảy ra khi tạo sản phẩm");
            }
        }

        public async Task<ApiResponse<ProductDto>> UpdateAsync(Guid id, ProductUpdateDto productUpdateDto)
        {
            try
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    return ApiResponse<ProductDto>.ErrorResult("Không tìm thấy sản phẩm");
                }

                // Validate business rules
                if (productUpdateDto.Price < productUpdateDto.StockPrice)
                {
                    return ApiResponse<ProductDto>.ErrorResult("Giá bán không được thấp hơn giá vốn");
                }

                _mapper.Map(productUpdateDto, existingProduct);
                var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
                var productDto = _mapper.Map<ProductDto>(updatedProduct);

                return ApiResponse<ProductDto>.SuccessResult(productDto, "Cập nhật sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm với ID: {ProductId}", id);
                return ApiResponse<ProductDto>.ErrorResult("Có lỗi xảy ra khi cập nhật sản phẩm");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var exists = await _productRepository.ExistsAsync(id);
                if (!exists)
                {
                    return ApiResponse<bool>.ErrorResult("Không tìm thấy sản phẩm");
                }

                var result = await _productRepository.DeleteAsync(id);
                return ApiResponse<bool>.SuccessResult(result, "Xóa sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm với ID: {ProductId}", id);
                return ApiResponse<bool>.ErrorResult("Có lỗi xảy ra khi xóa sản phẩm");
            }
        }

        public async Task<ApiResponse<List<ProductDto>>> GetByCategoryIdAsync(Guid categoryId)
        {
            try
            {
                var products = await _productRepository.GetByCategoryIdAsync(categoryId);
                var productDtos = _mapper.Map<List<ProductDto>>(products);

                return ApiResponse<List<ProductDto>>.SuccessResult(productDtos, "Lấy danh sách sản phẩm theo danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm theo danh mục với ID: {CategoryId}", categoryId);
                return ApiResponse<List<ProductDto>>.ErrorResult("Có lỗi xảy ra khi lấy danh sách sản phẩm theo danh mục");
            }
        }
    }
}
