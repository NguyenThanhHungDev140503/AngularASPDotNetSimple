using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Configuration
{
    /// <summary>
    /// Cấu hình logging với validation
    /// </summary>
    public class LoggingConfiguration
    {
        /// <summary>
        /// Log level mặc định
        /// </summary>
        [Required(ErrorMessage = "DefaultLogLevel là bắt buộc")]
        [RegularExpression(@"^(Trace|Debug|Information|Warning|Error|Critical|None)$", 
            ErrorMessage = "DefaultLogLevel phải là một trong: Trace, Debug, Information, Warning, Error, Critical, None")]
        public string DefaultLogLevel { get; set; } = "Information";

        /// <summary>
        /// Log level cho Microsoft.AspNetCore
        /// </summary>
        [RegularExpression(@"^(Trace|Debug|Information|Warning|Error|Critical|None)$", 
            ErrorMessage = "AspNetCoreLogLevel phải là một trong: Trace, Debug, Information, Warning, Error, Critical, None")]
        public string AspNetCoreLogLevel { get; set; } = "Warning";

        /// <summary>
        /// Log level cho Entity Framework
        /// </summary>
        [RegularExpression(@"^(Trace|Debug|Information|Warning|Error|Critical|None)$", 
            ErrorMessage = "EntityFrameworkLogLevel phải là một trong: Trace, Debug, Information, Warning, Error, Critical, None")]
        public string EntityFrameworkLogLevel { get; set; } = "Information";

        /// <summary>
        /// Có enable console logging không
        /// </summary>
        public bool EnableConsoleLogging { get; set; } = true;

        /// <summary>
        /// Có enable file logging không
        /// </summary>
        public bool EnableFileLogging { get; set; } = false;

        /// <summary>
        /// Đường dẫn file log
        /// </summary>
        public string? LogFilePath { get; set; }

        /// <summary>
        /// Kích thước tối đa của file log (MB)
        /// </summary>
        [Range(1, 1000, ErrorMessage = "MaxLogFileSizeMB phải từ 1 đến 1000 MB")]
        public int MaxLogFileSizeMB { get; set; } = 10;

        /// <summary>
        /// Số file log backup tối đa
        /// </summary>
        [Range(1, 100, ErrorMessage = "MaxLogFileCount phải từ 1 đến 100")]
        public int MaxLogFileCount { get; set; } = 5;

        /// <summary>
        /// Có enable structured logging không
        /// </summary>
        public bool EnableStructuredLogging { get; set; } = true;

        /// <summary>
        /// Có log request/response details không
        /// </summary>
        public bool LogRequestResponse { get; set; } = false;

        /// <summary>
        /// Có log SQL queries không (chỉ dùng trong development)
        /// </summary>
        public bool LogSqlQueries { get; set; } = false;
    }
}
