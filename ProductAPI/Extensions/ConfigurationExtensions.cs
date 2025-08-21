using Microsoft.Extensions.Options;
using ProductAPI.Configuration;
using ProductAPI.Services;

namespace ProductAPI.Extensions
{
    /// <summary>
    /// Extension methods cho configuration validation
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Thêm environment validation services vào DI container
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddEnvironmentValidation(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Register validation service
            services.AddScoped<IEnvironmentValidationService, EnvironmentValidationService>();

            // Configure and validate app configuration
            services.Configure<AppConfiguration>(config =>
            {
                // Bind database configuration
                configuration.GetSection("ConnectionStrings").Bind(config.Database, options =>
                {
                    options.BindNonPublicProperties = false;
                });
                
                // Map DefaultConnection to Database.DefaultConnection
                config.Database.DefaultConnection = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

                // Bind other configurations
                configuration.GetSection("Api").Bind(config.Api);
                configuration.GetSection("Security").Bind(config.Security);
                configuration.GetSection("Logging").Bind(config.Logging);

                // Set environment-specific defaults
                SetEnvironmentDefaults(config, configuration);
            });

            // Add validation on startup
            services.AddSingleton<IValidateOptions<AppConfiguration>, AppConfigurationValidator>();

            return services;
        }

        /// <summary>
        /// Validate configuration và throw exception nếu có lỗi
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection ValidateConfigurationOnStartup(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Build temporary service provider để validate
            using var serviceProvider = services.BuildServiceProvider();
            var validationService = serviceProvider.GetRequiredService<IEnvironmentValidationService>();
            var appConfig = serviceProvider.GetRequiredService<IOptions<AppConfiguration>>().Value;

            // Thực hiện validation
            var validationResult = validationService.ValidateConfiguration(appConfig);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors
                    .Select(e => $"- {e.Property}: {e.Message}")
                    .ToList();

                var warningMessages = validationResult.Warnings
                    .Select(w => $"- {w.Property}: {w.Message}")
                    .ToList();

                var message = "Configuration validation failed!\n\n";
                
                if (errorMessages.Any())
                {
                    message += "ERRORS:\n" + string.Join("\n", errorMessages) + "\n\n";
                }

                if (warningMessages.Any())
                {
                    message += "WARNINGS:\n" + string.Join("\n", warningMessages) + "\n\n";
                }

                message += "Please fix the configuration errors before starting the application.";

                throw new InvalidOperationException(message);
            }

            // Log warnings nếu có
            if (validationResult.Warnings.Any())
            {
                var logger = serviceProvider.GetRequiredService<ILogger<EnvironmentValidationService>>();
                foreach (var warning in validationResult.Warnings)
                {
                    logger.LogWarning("Configuration Warning - {Property}: {Message}", 
                        warning.Property, warning.Message);
                }
            }

            return services;
        }

        /// <summary>
        /// Test database connection và log kết quả
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static async Task<IServiceCollection> TestDatabaseConnectionAsync(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            using var serviceProvider = services.BuildServiceProvider();
            var validationService = serviceProvider.GetRequiredService<IEnvironmentValidationService>();
            var logger = serviceProvider.GetRequiredService<ILogger<EnvironmentValidationService>>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var canConnect = await validationService.TestDatabaseConnectionAsync(connectionString);
                if (canConnect)
                {
                    logger.LogInformation("Database connection test: SUCCESS");
                }
                else
                {
                    logger.LogWarning("Database connection test: FAILED - Application will continue but database operations may fail");
                }
            }

            return services;
        }

        /// <summary>
        /// Set environment-specific defaults
        /// </summary>
        private static void SetEnvironmentDefaults(AppConfiguration config, IConfiguration configuration)
        {
            var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
            config.Api.Environment = environment;

            // Production-specific defaults
            if (environment == "Production")
            {
                config.Api.EnableDetailedErrors = false;
                config.Database.EnableSensitiveDataLogging = false;
                config.Database.EnableDetailedErrors = false;
                config.Logging.LogSqlQueries = false;
                config.Logging.LogRequestResponse = false;
                config.Security.RequireHttps = true;
                config.Security.EnableHsts = true;
            }
            // Development-specific defaults
            else if (environment == "Development")
            {
                config.Api.EnableDetailedErrors = true;
                config.Database.EnableDetailedErrors = true;
                config.Logging.LogSqlQueries = true;
                config.Security.RequireHttps = false;
                config.Security.EnableHsts = false;
            }
        }
    }

    /// <summary>
    /// Validator cho AppConfiguration sử dụng IValidateOptions
    /// </summary>
    public class AppConfigurationValidator : IValidateOptions<AppConfiguration>
    {
        private readonly IEnvironmentValidationService _validationService;

        public AppConfigurationValidator(IEnvironmentValidationService validationService)
        {
            _validationService = validationService;
        }

        public ValidateOptionsResult Validate(string? name, AppConfiguration options)
        {
            var validationResult = _validationService.ValidateConfiguration(options);

            if (validationResult.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            var failures = validationResult.Errors
                .Select(e => $"{e.Property}: {e.Message}")
                .ToList();

            return ValidateOptionsResult.Fail(failures);
        }
    }
}
