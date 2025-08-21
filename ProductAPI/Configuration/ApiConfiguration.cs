using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Configuration
{
    /// <summary>
    /// Cấu hình API với validation
    /// </summary>
    public class ApiConfiguration
    {
        /// <summary>
        /// Tên ứng dụng
        /// </summary>
        [Required(ErrorMessage = "ApplicationName là bắt buộc")]
        [MinLength(3, ErrorMessage = "ApplicationName phải có ít nhất 3 ký tự")]
        [MaxLength(100, ErrorMessage = "ApplicationName không được vượt quá 100 ký tự")]
        public string ApplicationName { get; set; } = "Product API";

        /// <summary>
        /// Phiên bản API
        /// </summary>
        [Required(ErrorMessage = "Version là bắt buộc")]
        [RegularExpression(@"^\d+\.\d+\.\d+$", ErrorMessage = "Version phải có định dạng x.y.z (ví dụ: 1.0.0)")]
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Môi trường triển khai
        /// </summary>
        [Required(ErrorMessage = "Environment là bắt buộc")]
        [RegularExpression(@"^(Development|Staging|Production)$", 
            ErrorMessage = "Environment phải là một trong: Development, Staging, Production")]
        public string Environment { get; set; } = "Development";

        /// <summary>
        /// Base URL của API
        /// </summary>
        [Url(ErrorMessage = "BaseUrl phải là URL hợp lệ")]
        public string? BaseUrl { get; set; }

        /// <summary>
        /// Timeout cho HTTP requests (giây)
        /// </summary>
        [Range(1, 300, ErrorMessage = "RequestTimeout phải từ 1 đến 300 giây")]
        public int RequestTimeout { get; set; } = 30;

        /// <summary>
        /// Số lượng request tối đa mỗi phút
        /// </summary>
        [Range(1, 10000, ErrorMessage = "MaxRequestsPerMinute phải từ 1 đến 10000")]
        public int MaxRequestsPerMinute { get; set; } = 1000;

        /// <summary>
        /// Có enable detailed error responses không
        /// </summary>
        public bool EnableDetailedErrors { get; set; } = false;

        /// <summary>
        /// Có enable API versioning không
        /// </summary>
        public bool EnableVersioning { get; set; } = true;

        /// <summary>
        /// Có enable compression không
        /// </summary>
        public bool EnableCompression { get; set; } = true;
    }
}
