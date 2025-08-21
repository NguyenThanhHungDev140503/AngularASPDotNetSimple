using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Configuration
{
    /// <summary>
    /// Cấu hình bảo mật với validation
    /// </summary>
    public class SecurityConfiguration
    {
        /// <summary>
        /// Secret key cho JWT token
        /// </summary>
        [Required(ErrorMessage = "JwtSecretKey là bắt buộc")]
        [MinLength(32, ErrorMessage = "JwtSecretKey phải có ít nhất 32 ký tự để đảm bảo bảo mật")]
        public string JwtSecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian hết hạn của JWT token (phút)
        /// </summary>
        [Range(1, 1440, ErrorMessage = "JwtExpirationMinutes phải từ 1 đến 1440 phút (24 giờ)")]
        public int JwtExpirationMinutes { get; set; } = 60;

        /// <summary>
        /// Issuer của JWT token
        /// </summary>
        [Required(ErrorMessage = "JwtIssuer là bắt buộc")]
        [MinLength(3, ErrorMessage = "JwtIssuer phải có ít nhất 3 ký tự")]
        public string JwtIssuer { get; set; } = "ProductAPI";

        /// <summary>
        /// Audience của JWT token
        /// </summary>
        [Required(ErrorMessage = "JwtAudience là bắt buộc")]
        [MinLength(3, ErrorMessage = "JwtAudience phải có ít nhất 3 ký tự")]
        public string JwtAudience { get; set; } = "ProductAPI.Client";

        /// <summary>
        /// Danh sách CORS origins được phép
        /// </summary>
        public List<string> AllowedOrigins { get; set; } = new List<string> { "http://localhost:4200" };

        /// <summary>
        /// Có enable HTTPS redirection không
        /// </summary>
        public bool RequireHttps { get; set; } = true;

        /// <summary>
        /// Có enable HSTS không
        /// </summary>
        public bool EnableHsts { get; set; } = true;

        /// <summary>
        /// Thời gian HSTS max age (giây)
        /// </summary>
        [Range(300, 31536000, ErrorMessage = "HstsMaxAge phải từ 300 giây (5 phút) đến 31536000 giây (1 năm)")]
        public int HstsMaxAge { get; set; } = 31536000; // 1 năm

        /// <summary>
        /// Có enable rate limiting không
        /// </summary>
        public bool EnableRateLimiting { get; set; } = true;

        /// <summary>
        /// Số request tối đa mỗi IP trong 1 phút
        /// </summary>
        [Range(1, 1000, ErrorMessage = "RateLimitPerMinute phải từ 1 đến 1000")]
        public int RateLimitPerMinute { get; set; } = 100;
    }
}
