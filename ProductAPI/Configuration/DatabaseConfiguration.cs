using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Configuration
{
    /// <summary>
    /// Cấu hình cơ sở dữ liệu với validation
    /// </summary>
    public class DatabaseConfiguration
    {
        /// <summary>
        /// Connection string cho cơ sở dữ liệu chính
        /// </summary>
        [Required(ErrorMessage = "DefaultConnection là bắt buộc")]
        [MinLength(10, ErrorMessage = "DefaultConnection phải có ít nhất 10 ký tự")]
        public string DefaultConnection { get; set; } = string.Empty;

        /// <summary>
        /// Timeout cho database command (giây)
        /// </summary>
        [Range(1, 300, ErrorMessage = "CommandTimeout phải từ 1 đến 300 giây")]
        public int CommandTimeout { get; set; } = 30;

        /// <summary>
        /// Số lần retry khi kết nối database thất bại
        /// </summary>
        [Range(0, 10, ErrorMessage = "MaxRetryCount phải từ 0 đến 10")]
        public int MaxRetryCount { get; set; } = 3;

        /// <summary>
        /// Thời gian delay giữa các lần retry (milliseconds)
        /// </summary>
        [Range(100, 10000, ErrorMessage = "RetryDelay phải từ 100 đến 10000 milliseconds")]
        public int RetryDelay { get; set; } = 1000;

        /// <summary>
        /// Có enable sensitive data logging không
        /// </summary>
        public bool EnableSensitiveDataLogging { get; set; } = false;

        /// <summary>
        /// Có enable detailed errors không (chỉ dùng trong development)
        /// </summary>
        public bool EnableDetailedErrors { get; set; } = false;
    }
}
