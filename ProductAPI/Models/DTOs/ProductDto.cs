using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Models.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal StockPrice { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid? CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(int.MaxValue, ErrorMessage = "Mô tả quá dài")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá vốn là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá vốn phải lớn hơn hoặc bằng 0")]
        public decimal StockPrice { get; set; }

        [Required(ErrorMessage = "Giá bán là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int StockQuantity { get; set; }

        public Guid? CategoryId { get; set; }
    }

    public class ProductUpdateDto
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(int.MaxValue, ErrorMessage = "Mô tả quá dài")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá vốn là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá vốn phải lớn hơn hoặc bằng 0")]
        public decimal StockPrice { get; set; }

        [Required(ErrorMessage = "Giá bán là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int StockQuantity { get; set; }

        public Guid? CategoryId { get; set; }
    }

    public class ProductQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } = "Name";
        public string? SortOrder { get; set; } = "asc";
    }
}
