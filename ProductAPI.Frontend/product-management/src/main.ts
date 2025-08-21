import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { environment } from './environments/environment';
import { EnvironmentValidationService } from './app/core/services/environment-validation.service';

/**
 * Validate environment configuration trước khi bootstrap app
 */
function validateEnvironment(): void {
  const validationService = new EnvironmentValidationService();

  // Configure validation
  validationService.configure({
    validateOnStartup: true,
    throwOnError: true,
    logWarnings: true,
    enableRuntimeValidation: false
  });

  // Validate environment
  const validationResult = validationService.validateEnvironment(environment);

  // Log warnings
  if (validationResult.warnings.length > 0) {
    console.warn('🟡 Environment Configuration Warnings:');
    validationResult.warnings.forEach(warning => {
      console.warn(`- ${warning.property}: ${warning.message}`);
    });
    console.warn(''); // Empty line
  }

  // Handle errors
  if (!validationResult.isValid) {
    console.error('❌ Environment Configuration Errors:');
    validationResult.errors.forEach(error => {
      console.error(`- ${error.property}: ${error.message}`);
    });

    const errorMessage = `
🚨 APPLICATION STARTUP FAILED 🚨

Environment configuration validation failed!

${validationService.formatValidationResult(validationResult)}

Please fix the configuration errors before starting the application.

Current environment: ${environment.production ? 'PRODUCTION' : 'DEVELOPMENT'}
`;

    // Show user-friendly error
    document.body.innerHTML = `
      <div style="
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        max-width: 800px;
        margin: 50px auto;
        padding: 30px;
        background: #fff;
        border-radius: 8px;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        border-left: 5px solid #dc3545;
      ">
        <h1 style="color: #dc3545; margin-bottom: 20px;">
          ⚠️ Configuration Error
        </h1>
        <p style="color: #666; margin-bottom: 20px;">
          The application cannot start due to invalid environment configuration.
        </p>
        <pre style="
          background: #f8f9fa;
          padding: 15px;
          border-radius: 4px;
          overflow-x: auto;
          white-space: pre-wrap;
          font-size: 14px;
          color: #495057;
        ">${validationService.formatValidationResult(validationResult)}</pre>
        <p style="color: #666; margin-top: 20px;">
          Please contact the development team to fix these configuration issues.
        </p>
      </div>
    `;

    throw new Error(errorMessage);
  }

  // Success message
  console.log('✅ Environment configuration validation passed');
  if (validationResult.warnings.length === 0) {
    console.log('✅ No warnings found');
  }
}

// Validate environment before bootstrapping
try {
  validateEnvironment();

  // Bootstrap application
  bootstrapApplication(App, appConfig)
    .then(() => {
      console.log('🚀 Application started successfully');
    })
    .catch((err) => {
      console.error('❌ Application bootstrap failed:', err);

      // Show user-friendly error for bootstrap failures
      document.body.innerHTML = `
        <div style="
          font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
          max-width: 800px;
          margin: 50px auto;
          padding: 30px;
          background: #fff;
          border-radius: 8px;
          box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
          border-left: 5px solid #dc3545;
        ">
          <h1 style="color: #dc3545; margin-bottom: 20px;">
            ⚠️ Application Startup Error
          </h1>
          <p style="color: #666; margin-bottom: 20px;">
            The application failed to start due to an unexpected error.
          </p>
          <pre style="
            background: #f8f9fa;
            padding: 15px;
            border-radius: 4px;
            overflow-x: auto;
            white-space: pre-wrap;
            font-size: 14px;
            color: #495057;
          ">${err}</pre>
          <p style="color: #666; margin-top: 20px;">
            Please contact the development team for assistance.
          </p>
        </div>
      `;
    });

} catch (validationError) {
  console.error('❌ Environment validation failed:', validationError);
  // Error page already shown in validateEnvironment function
}
