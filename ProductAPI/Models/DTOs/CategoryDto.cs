using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Models.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(int.MaxValue, ErrorMessage = "Mô tả quá dài")]
        public string? Description { get; set; }
    }

    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(int.MaxValue, ErrorMessage = "Mô tả quá dài")]
        public string? Description { get; set; }
    }

    public class CategoryQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "Name";
        public string? SortOrder { get; set; } = "asc";
    }
}
