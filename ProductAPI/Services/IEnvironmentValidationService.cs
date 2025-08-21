using ProductAPI.Configuration;

namespace ProductAPI.Services
{
    /// <summary>
    /// Interface cho service validation environment configuration
    /// </summary>
    public interface IEnvironmentValidationService
    {
        /// <summary>
        /// Validate toàn bộ cấu hình ứng dụng
        /// </summary>
        /// <param name="configuration">Cấu hình cần validate</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateConfiguration(AppConfiguration configuration);

        /// <summary>
        /// Validate cấu hình cơ sở dữ liệu
        /// </summary>
        /// <param name="config">Cấu hình database</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateDatabaseConfiguration(DatabaseConfiguration config);

        /// <summary>
        /// Validate cấu hình API
        /// </summary>
        /// <param name="config">Cấu hình API</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateApiConfiguration(ApiConfiguration config);

        /// <summary>
        /// Validate cấu hình bảo mật
        /// </summary>
        /// <param name="config">Cấu hình security</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateSecurityConfiguration(SecurityConfiguration config);

        /// <summary>
        /// Validate cấu hình logging
        /// </summary>
        /// <param name="config">Cấu hình logging</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateLoggingConfiguration(LoggingConfiguration config);

        /// <summary>
        /// Kiểm tra kết nối database
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>True nếu kết nối thành công</returns>
        Task<bool> TestDatabaseConnectionAsync(string connectionString);
    }

    /// <summary>
    /// Kết quả validation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Có hợp lệ không
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Danh sách lỗi
        /// </summary>
        public List<ValidationError> Errors { get; set; } = new();

        /// <summary>
        /// Danh sách cảnh báo
        /// </summary>
        public List<ValidationWarning> Warnings { get; set; } = new();

        /// <summary>
        /// Thêm lỗi validation
        /// </summary>
        public void AddError(string property, string message, object? value = null)
        {
            Errors.Add(new ValidationError
            {
                Property = property,
                Message = message,
                Value = value
            });
            IsValid = false;
        }

        /// <summary>
        /// Thêm cảnh báo validation
        /// </summary>
        public void AddWarning(string property, string message, object? value = null)
        {
            Warnings.Add(new ValidationWarning
            {
                Property = property,
                Message = message,
                Value = value
            });
        }

        /// <summary>
        /// Merge kết quả validation khác
        /// </summary>
        public void Merge(ValidationResult other)
        {
            Errors.AddRange(other.Errors);
            Warnings.AddRange(other.Warnings);
            IsValid = IsValid && other.IsValid;
        }
    }

    /// <summary>
    /// Lỗi validation
    /// </summary>
    public class ValidationError
    {
        public string Property { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Value { get; set; }
    }

    /// <summary>
    /// Cảnh báo validation
    /// </summary>
    public class ValidationWarning
    {
        public string Property { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Value { get; set; }
    }
}
