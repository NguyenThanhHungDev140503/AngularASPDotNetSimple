using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Configuration;
using ProductAPI.Services;
using Xunit;

namespace ProductAPI.Tests
{
    public class EnvironmentValidationServiceTests
    {
        private readonly Mock<ILogger<EnvironmentValidationService>> _mockLogger;
        private readonly EnvironmentValidationService _validationService;

        public EnvironmentValidationServiceTests()
        {
            _mockLogger = new Mock<ILogger<EnvironmentValidationService>>();
            _validationService = new EnvironmentValidationService(_mockLogger.Object);
        }

        #region Database Configuration Tests

        [Fact]
        public void ValidateDatabaseConfiguration_ValidConfig_ReturnsValid()
        {
            // Arrange
            var config = new DatabaseConfiguration
            {
                DefaultConnection = "Server=localhost;Database=TestDB;Trusted_Connection=true;TrustServerCertificate=true",
                CommandTimeout = 30,
                MaxRetryCount = 3,
                RetryDelay = 1000,
                EnableSensitiveDataLogging = false,
                EnableDetailedErrors = false
            };

            // Act
            var result = _validationService.ValidateDatabaseConfiguration(config);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidateDatabaseConfiguration_EmptyConnectionString_ReturnsError()
        {
            // Arrange
            var config = new DatabaseConfiguration
            {
                DefaultConnection = "",
                CommandTimeout = 30,
                MaxRetryCount = 3,
                RetryDelay = 1000
            };

            // Act
            var result = _validationService.ValidateDatabaseConfiguration(config);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Property == "DefaultConnection");
        }

        [Fact]
        public void ValidateDatabaseConfiguration_InvalidCommandTimeout_ReturnsError()
        {
            // Arrange
            var config = new DatabaseConfiguration
            {
                DefaultConnection = "Server=localhost;Database=TestDB;Trusted_Connection=true",
                CommandTimeout = 0, // Invalid
                MaxRetryCount = 3,
                RetryDelay = 1000
            };

            // Act
            var result = _validationService.ValidateDatabaseConfiguration(config);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Property == "CommandTimeout");
        }

        [Fact]
        public void ValidateDatabaseConfiguration_HighCommandTimeout_ReturnsWarning()
        {
            // Arrange
            var config = new DatabaseConfiguration
            {
                DefaultConnection = "Server=localhost;Database=TestDB;Trusted_Connection=true",
                CommandTimeout = 150, // High value
                MaxRetryCount = 3,
                RetryDelay = 1000
            };

            // Act
            var result = _validationService.ValidateDatabaseConfiguration(config);

            // Assert
            Assert.True(result.IsValid);
            Assert.Contains(result.Warnings, w => w.Property == "CommandTimeout");
        }

        #endregion

        #region API Configuration Tests

        [Fact]
        public void ValidateApiConfiguration_ValidConfig_ReturnsValid()
        {
            // Arrange
            var config = new ApiConfiguration
            {
                ApplicationName = "Test API",
                Version = "1.0.0",
                Environment = "Development",
                BaseUrl = "http://localhost:5000",
                RequestTimeout = 30,
                MaxRequestsPerMinute = 1000,
                EnableDetailedErrors = true
            };

            // Act
            var result = _validationService.ValidateApiConfiguration(config);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidateApiConfiguration_InvalidVersion_ReturnsError()
        {
            // Arrange
            var config = new ApiConfiguration
            {
                ApplicationName = "Test API",
                Version = "invalid-version", // Invalid format
                Environment = "Development",
                RequestTimeout = 30,
                MaxRequestsPerMinute = 1000
            };

            // Act
            var result = _validationService.ValidateApiConfiguration(config);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Property == "Version");
        }

        [Fact]
        public void ValidateApiConfiguration_InvalidEnvironment_ReturnsError()
        {
            // Arrange
            var config = new ApiConfiguration
            {
                ApplicationName = "Test API",
                Version = "1.0.0",
                Environment = "InvalidEnv", // Invalid environment
                RequestTimeout = 30,
                MaxRequestsPerMinute = 1000
            };

            // Act
            var result = _validationService.ValidateApiConfiguration(config);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Property == "Environment");
        }

        [Fact]
        public void ValidateApiConfiguration_ProductionWithHttpUrl_ReturnsWarning()
        {
            // Arrange
            var config = new ApiConfiguration
            {
                ApplicationName = "Test API",
                Version = "1.0.0",
                Environment = "Production",
                BaseUrl = "http://example.com", // HTTP in production
                RequestTimeout = 30,
                MaxRequestsPerMinute = 1000
            };

            // Act
            var result = _validationService.ValidateApiConfiguration(config);

            // Assert
            Assert.True(result.IsValid);
            Assert.Contains(result.Warnings, w => w.Property == "BaseUrl");
        }

        #endregion

        #region Security Configuration Tests

        [Fact]
        public void ValidateSecurityConfiguration_ValidConfig_ReturnsValid()
        {
            // Arrange
            var config = new SecurityConfiguration
            {
                JwtSecretKey = "this-is-a-very-secure-secret-key-with-at-least-32-characters",
                JwtExpirationMinutes = 60,
                JwtIssuer = "TestAPI",
                JwtAudience = "TestAPI.Client",
                AllowedOrigins = new List<string> { "https://example.com" },
                RequireHttps = true,
                EnableHsts = true,
                RateLimitPerMinute = 100
            };

            // Act
            var result = _validationService.ValidateSecurityConfiguration(config);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidateSecurityConfiguration_ShortSecretKey_ReturnsError()
        {
            // Arrange
            var config = new SecurityConfiguration
            {
                JwtSecretKey = "short", // Too short
                JwtExpirationMinutes = 60,
                JwtIssuer = "TestAPI",
                JwtAudience = "TestAPI.Client",
                AllowedOrigins = new List<string> { "https://example.com" }
            };

            // Act
            var result = _validationService.ValidateSecurityConfiguration(config);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Property == "JwtSecretKey");
        }

        [Fact]
        public void ValidateSecurityConfiguration_WeakSecretKey_ReturnsError()
        {
            // Arrange
            var config = new SecurityConfiguration
            {
                JwtSecretKey = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", // All same character
                JwtExpirationMinutes = 60,
                JwtIssuer = "TestAPI",
                JwtAudience = "TestAPI.Client",
                AllowedOrigins = new List<string> { "https://example.com" }
            };

            // Act
            var result = _validationService.ValidateSecurityConfiguration(config);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Property == "JwtSecretKey");
        }

        [Fact]
        public void ValidateSecurityConfiguration_InvalidCorsOrigin_ReturnsError()
        {
            // Arrange
            var config = new SecurityConfiguration
            {
                JwtSecretKey = "this-is-a-very-secure-secret-key-with-at-least-32-characters",
                JwtExpirationMinutes = 60,
                JwtIssuer = "TestAPI",
                JwtAudience = "TestAPI.Client",
                AllowedOrigins = new List<string> { "invalid-url" }, // Invalid URL
                RequireHttps = true
            };

            // Act
            var result = _validationService.ValidateSecurityConfiguration(config);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Property == "AllowedOrigins");
        }

        #endregion

        #region Logging Configuration Tests

        [Fact]
        public void ValidateLoggingConfiguration_ValidConfig_ReturnsValid()
        {
            // Arrange
            var config = new LoggingConfiguration
            {
                DefaultLogLevel = "Information",
                AspNetCoreLogLevel = "Warning",
                EntityFrameworkLogLevel = "Information",
                EnableConsoleLogging = true,
                EnableFileLogging = false,
                MaxLogFileSizeMB = 10,
                MaxLogFileCount = 5,
                LogSqlQueries = false
            };

            // Act
            var result = _validationService.ValidateLoggingConfiguration(config);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidateLoggingConfiguration_InvalidLogLevel_ReturnsError()
        {
            // Arrange
            var config = new LoggingConfiguration
            {
                DefaultLogLevel = "InvalidLevel", // Invalid log level
                AspNetCoreLogLevel = "Warning",
                EntityFrameworkLogLevel = "Information",
                EnableConsoleLogging = true
            };

            // Act
            var result = _validationService.ValidateLoggingConfiguration(config);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Property == "DefaultLogLevel");
        }

        [Fact]
        public void ValidateLoggingConfiguration_FileLoggingWithoutPath_ReturnsError()
        {
            // Arrange
            var config = new LoggingConfiguration
            {
                DefaultLogLevel = "Information",
                EnableFileLogging = true,
                LogFilePath = null // Missing path when file logging enabled
            };

            // Act
            var result = _validationService.ValidateLoggingConfiguration(config);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Property == "LogFilePath");
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void ValidateConfiguration_ValidAppConfig_ReturnsValid()
        {
            // Arrange
            var appConfig = new AppConfiguration
            {
                Database = new DatabaseConfiguration
                {
                    DefaultConnection = "Server=localhost;Database=TestDB;Trusted_Connection=true",
                    CommandTimeout = 30,
                    MaxRetryCount = 3,
                    RetryDelay = 1000
                },
                Api = new ApiConfiguration
                {
                    ApplicationName = "Test API",
                    Version = "1.0.0",
                    Environment = "Development",
                    RequestTimeout = 30,
                    MaxRequestsPerMinute = 1000
                },
                Security = new SecurityConfiguration
                {
                    JwtSecretKey = "this-is-a-very-secure-secret-key-with-at-least-32-characters",
                    JwtExpirationMinutes = 60,
                    JwtIssuer = "TestAPI",
                    JwtAudience = "TestAPI.Client",
                    AllowedOrigins = new List<string> { "https://example.com" }
                },
                Logging = new LoggingConfiguration
                {
                    DefaultLogLevel = "Information",
                    EnableConsoleLogging = true,
                    EnableFileLogging = false
                }
            };

            // Act
            var result = _validationService.ValidateConfiguration(appConfig);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateConfiguration_ProductionWithInsecureSettings_ReturnsErrors()
        {
            // Arrange
            var appConfig = new AppConfiguration
            {
                Api = new ApiConfiguration
                {
                    ApplicationName = "Test API",
                    Version = "1.0.0",
                    Environment = "Production", // Production environment
                    EnableDetailedErrors = true // Should be false in production
                },
                Security = new SecurityConfiguration
                {
                    JwtSecretKey = "secure-key-with-at-least-32-characters",
                    JwtExpirationMinutes = 60,
                    JwtIssuer = "TestAPI",
                    JwtAudience = "TestAPI.Client",
                    RequireHttps = false, // Should be true in production
                    AllowedOrigins = new List<string> { "https://example.com" }
                },
                Database = new DatabaseConfiguration
                {
                    DefaultConnection = "Server=localhost;Database=TestDB;Trusted_Connection=true",
                    EnableSensitiveDataLogging = true // Should be false in production
                },
                Logging = new LoggingConfiguration
                {
                    DefaultLogLevel = "Information",
                    LogSqlQueries = true // Should be false in production
                }
            };

            // Act
            var result = _validationService.ValidateConfiguration(appConfig);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Property == "Security.RequireHttps");
            Assert.Contains(result.Errors, e => e.Property == "Api.EnableDetailedErrors");
            Assert.Contains(result.Errors, e => e.Property == "Database.EnableSensitiveDataLogging");
            Assert.Contains(result.Errors, e => e.Property == "Logging.LogSqlQueries");
        }

        #endregion
    }
}
