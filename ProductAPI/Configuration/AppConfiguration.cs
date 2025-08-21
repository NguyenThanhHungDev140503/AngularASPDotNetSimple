namespace ProductAPI.Configuration
{
    /// <summary>
    /// Tổng hợp tất cả cấu hình của ứng dụng
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Cấu hình cơ sở dữ liệu
        /// </summary>
        public DatabaseConfiguration Database { get; set; } = new();

        /// <summary>
        /// Cấu hình API
        /// </summary>
        public ApiConfiguration Api { get; set; } = new();

        /// <summary>
        /// Cấu hình bảo mật
        /// </summary>
        public SecurityConfiguration Security { get; set; } = new();

        /// <summary>
        /// Cấu hình logging
        /// </summary>
        public LoggingConfiguration Logging { get; set; } = new();
    }
}
