using Microsoft.Data.SqlClient;
using ProductAPI.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ProductAPI.Services
{
    /// <summary>
    /// Service validation environment configuration
    /// </summary>
    public class EnvironmentValidationService : IEnvironmentValidationService
    {
        private readonly ILogger<EnvironmentValidationService> _logger;

        public EnvironmentValidationService(ILogger<EnvironmentValidationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validate toàn bộ cấu hình ứng dụng
        /// </summary>
        public ValidationResult ValidateConfiguration(AppConfiguration configuration)
        {
            var result = new ValidationResult { IsValid = true };

            try
            {
                // Validate từng phần cấu hình
                result.Merge(ValidateDatabaseConfiguration(configuration.Database));
                result.Merge(ValidateApiConfiguration(configuration.Api));
                result.Merge(ValidateSecurityConfiguration(configuration.Security));
                result.Merge(ValidateLoggingConfiguration(configuration.Logging));

                // Validate cross-configuration dependencies
                ValidateCrossConfigurationDependencies(configuration, result);

                _logger.LogInformation("Configuration validation completed. IsValid: {IsValid}, Errors: {ErrorCount}, Warnings: {WarningCount}",
                    result.IsValid, result.Errors.Count, result.Warnings.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during configuration validation");
                result.AddError("Configuration", $"Unexpected error during validation: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Validate cấu hình cơ sở dữ liệu
        /// </summary>
        public ValidationResult ValidateDatabaseConfiguration(DatabaseConfiguration config)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate using Data Annotations
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new ValidationContext(config);
            
            if (!Validator.TryValidateObject(config, context, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    result.AddError(
                        validationResult.MemberNames.FirstOrDefault() ?? "Unknown",
                        validationResult.ErrorMessage ?? "Validation error"
                    );
                }
            }

            // Custom validations
            ValidateConnectionString(config.DefaultConnection, result);
            ValidateDatabaseSettings(config, result);

            return result;
        }

        /// <summary>
        /// Validate connection string format
        /// </summary>
        private void ValidateConnectionString(string connectionString, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                result.AddError("DefaultConnection", "Connection string không được để trống");
                return;
            }

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                
                // Kiểm tra các thành phần bắt buộc
                if (string.IsNullOrWhiteSpace(builder.DataSource))
                {
                    result.AddError("DefaultConnection", "Connection string phải chứa Server/Data Source");
                }

                if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
                {
                    result.AddError("DefaultConnection", "Connection string phải chứa Database/Initial Catalog");
                }

                // Cảnh báo về bảo mật
                if (builder.IntegratedSecurity == false && 
                    (string.IsNullOrWhiteSpace(builder.UserID) || string.IsNullOrWhiteSpace(builder.Password)))
                {
                    result.AddWarning("DefaultConnection", 
                        "Connection string không sử dụng Integrated Security và thiếu User ID/Password");
                }

                if (!builder.TrustServerCertificate && !builder.Encrypt)
                {
                    result.AddWarning("DefaultConnection", 
                        "Nên enable TrustServerCertificate=true hoặc Encrypt=true để bảo mật");
                }
            }
            catch (Exception ex)
            {
                result.AddError("DefaultConnection", $"Connection string không hợp lệ: {ex.Message}");
            }
        }

        /// <summary>
        /// Validate database settings
        /// </summary>
        private void ValidateDatabaseSettings(DatabaseConfiguration config, ValidationResult result)
        {
            // Cảnh báo về performance
            if (config.CommandTimeout > 120)
            {
                result.AddWarning("CommandTimeout",
                    "CommandTimeout > 120 giây có thể ảnh hưởng đến performance");
            }

            if (config.MaxRetryCount > 5)
            {
                result.AddWarning("MaxRetryCount",
                    "MaxRetryCount > 5 có thể gây delay quá lâu khi database không khả dụng");
            }

            // Cảnh báo về security trong production
            if (config.EnableSensitiveDataLogging)
            {
                result.AddWarning("EnableSensitiveDataLogging",
                    "Không nên enable SensitiveDataLogging trong production");
            }

            if (config.EnableDetailedErrors)
            {
                result.AddWarning("EnableDetailedErrors",
                    "Không nên enable DetailedErrors trong production");
            }
        }

        /// <summary>
        /// Validate cấu hình API
        /// </summary>
        public ValidationResult ValidateApiConfiguration(ApiConfiguration config)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate using Data Annotations
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new ValidationContext(config);

            if (!Validator.TryValidateObject(config, context, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    result.AddError(
                        validationResult.MemberNames.FirstOrDefault() ?? "Unknown",
                        validationResult.ErrorMessage ?? "Validation error"
                    );
                }
            }

            // Custom validations
            ValidateApiSettings(config, result);

            return result;
        }

        /// <summary>
        /// Validate API settings
        /// </summary>
        private void ValidateApiSettings(ApiConfiguration config, ValidationResult result)
        {
            // Validate BaseUrl if provided
            if (!string.IsNullOrWhiteSpace(config.BaseUrl))
            {
                if (!Uri.TryCreate(config.BaseUrl, UriKind.Absolute, out var uri))
                {
                    result.AddError("BaseUrl", "BaseUrl không phải là URL hợp lệ");
                }
                else if (uri.Scheme != "https" && config.Environment == "Production")
                {
                    result.AddWarning("BaseUrl", "Production environment nên sử dụng HTTPS");
                }
            }

            // Performance warnings
            if (config.RequestTimeout > 120)
            {
                result.AddWarning("RequestTimeout",
                    "RequestTimeout > 120 giây có thể gây timeout cho client");
            }

            if (config.MaxRequestsPerMinute > 5000)
            {
                result.AddWarning("MaxRequestsPerMinute",
                    "MaxRequestsPerMinute > 5000 có thể gây quá tải server");
            }

            // Security warnings
            if (config.EnableDetailedErrors && config.Environment == "Production")
            {
                result.AddWarning("EnableDetailedErrors",
                    "Không nên enable DetailedErrors trong production");
            }
        }

        /// <summary>
        /// Validate cấu hình bảo mật
        /// </summary>
        public ValidationResult ValidateSecurityConfiguration(SecurityConfiguration config)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate using Data Annotations
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new ValidationContext(config);

            if (!Validator.TryValidateObject(config, context, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    result.AddError(
                        validationResult.MemberNames.FirstOrDefault() ?? "Unknown",
                        validationResult.ErrorMessage ?? "Validation error"
                    );
                }
            }

            // Custom validations
            ValidateSecuritySettings(config, result);

            return result;
        }

        /// <summary>
        /// Validate security settings
        /// </summary>
        private void ValidateSecuritySettings(SecurityConfiguration config, ValidationResult result)
        {
            // Validate JWT Secret Key strength
            if (!string.IsNullOrWhiteSpace(config.JwtSecretKey))
            {
                if (config.JwtSecretKey.Length < 64)
                {
                    result.AddWarning("JwtSecretKey",
                        "JWT Secret Key nên có ít nhất 64 ký tự để bảo mật tốt hơn");
                }

                // Check for weak patterns
                if (Regex.IsMatch(config.JwtSecretKey, @"^(.)\1+$")) // All same character
                {
                    result.AddError("JwtSecretKey", "JWT Secret Key không được chứa toàn ký tự giống nhau");
                }

                if (config.JwtSecretKey.ToLower().Contains("secret") ||
                    config.JwtSecretKey.ToLower().Contains("password"))
                {
                    result.AddWarning("JwtSecretKey",
                        "JWT Secret Key không nên chứa từ 'secret' hoặc 'password'");
                }
            }

            // Validate CORS origins
            foreach (var origin in config.AllowedOrigins)
            {
                if (origin == "*")
                {
                    result.AddWarning("AllowedOrigins",
                        "Không nên sử dụng wildcard (*) cho CORS origins trong production");
                    continue;
                }

                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    result.AddError("AllowedOrigins", $"CORS origin không hợp lệ: {origin}");
                }
                else if (uri.Scheme != "https" && !origin.StartsWith("http://localhost"))
                {
                    result.AddWarning("AllowedOrigins",
                        $"CORS origin nên sử dụng HTTPS: {origin}");
                }
            }

            // Security recommendations
            if (!config.RequireHttps)
            {
                result.AddWarning("RequireHttps",
                    "Nên enable HTTPS redirection để bảo mật");
            }

            if (!config.EnableHsts)
            {
                result.AddWarning("EnableHsts",
                    "Nên enable HSTS để bảo mật");
            }

            if (config.RateLimitPerMinute > 500)
            {
                result.AddWarning("RateLimitPerMinute",
                    "Rate limit > 500 requests/minute có thể không đủ để chống DDoS");
            }
        }

        /// <summary>
        /// Validate cấu hình logging
        /// </summary>
        public ValidationResult ValidateLoggingConfiguration(LoggingConfiguration config)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate using Data Annotations
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new ValidationContext(config);

            if (!Validator.TryValidateObject(config, context, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    result.AddError(
                        validationResult.MemberNames.FirstOrDefault() ?? "Unknown",
                        validationResult.ErrorMessage ?? "Validation error"
                    );
                }
            }

            // Custom validations
            ValidateLoggingSettings(config, result);

            return result;
        }

        /// <summary>
        /// Validate logging settings
        /// </summary>
        private void ValidateLoggingSettings(LoggingConfiguration config, ValidationResult result)
        {
            // Validate log file path if file logging is enabled
            if (config.EnableFileLogging)
            {
                if (string.IsNullOrWhiteSpace(config.LogFilePath))
                {
                    result.AddError("LogFilePath",
                        "LogFilePath là bắt buộc khi EnableFileLogging = true");
                }
                else
                {
                    try
                    {
                        var directory = Path.GetDirectoryName(config.LogFilePath);
                        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                        {
                            result.AddWarning("LogFilePath",
                                $"Thư mục log không tồn tại: {directory}");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.AddError("LogFilePath",
                            $"LogFilePath không hợp lệ: {ex.Message}");
                    }
                }
            }

            // Performance warnings
            if (config.MaxLogFileSizeMB > 100)
            {
                result.AddWarning("MaxLogFileSizeMB",
                    "Log file size > 100MB có thể ảnh hưởng đến performance");
            }

            if (config.MaxLogFileCount > 20)
            {
                result.AddWarning("MaxLogFileCount",
                    "Số lượng log file > 20 có thể chiếm nhiều disk space");
            }

            // Security warnings
            if (config.LogSqlQueries)
            {
                result.AddWarning("LogSqlQueries",
                    "Không nên log SQL queries trong production (có thể chứa sensitive data)");
            }

            if (config.LogRequestResponse)
            {
                result.AddWarning("LogRequestResponse",
                    "Cẩn thận khi log request/response (có thể chứa sensitive data)");
            }

            // Log level recommendations
            if (config.DefaultLogLevel == "Trace" || config.DefaultLogLevel == "Debug")
            {
                result.AddWarning("DefaultLogLevel",
                    "Trace/Debug log level có thể ảnh hưởng đến performance trong production");
            }
        }

        /// <summary>
        /// Validate cross-configuration dependencies
        /// </summary>
        private void ValidateCrossConfigurationDependencies(AppConfiguration config, ValidationResult result)
        {
            // Security vs Environment consistency
            if (config.Api.Environment == "Production")
            {
                if (!config.Security.RequireHttps)
                {
                    result.AddError("Security.RequireHttps",
                        "Production environment phải enable HTTPS");
                }

                if (config.Api.EnableDetailedErrors)
                {
                    result.AddError("Api.EnableDetailedErrors",
                        "Production environment không được enable detailed errors");
                }

                if (config.Database.EnableSensitiveDataLogging)
                {
                    result.AddError("Database.EnableSensitiveDataLogging",
                        "Production environment không được enable sensitive data logging");
                }

                if (config.Logging.LogSqlQueries)
                {
                    result.AddError("Logging.LogSqlQueries",
                        "Production environment không được log SQL queries");
                }
            }

            // JWT configuration consistency
            if (config.Security.JwtExpirationMinutes > 480) // 8 hours
            {
                result.AddWarning("Security.JwtExpirationMinutes",
                    "JWT expiration > 8 giờ có thể không an toàn");
            }

            // Performance consistency
            if (config.Database.CommandTimeout > config.Api.RequestTimeout)
            {
                result.AddWarning("Database.CommandTimeout",
                    "Database CommandTimeout nên nhỏ hơn API RequestTimeout");
            }
        }

        /// <summary>
        /// Kiểm tra kết nối database
        /// </summary>
        public async Task<bool> TestDatabaseConnectionAsync(string connectionString)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection test failed");
                return false;
            }
        }
    }
}
