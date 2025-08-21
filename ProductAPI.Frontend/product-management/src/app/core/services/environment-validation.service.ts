import { Injectable } from '@angular/core';
import { 
  Environment, 
  ValidationResult, 
  ValidationError, 
  ValidationWarning,
  ValidationRule,
  ValidationMetadata,
  ENVIRONMENT_VALIDATION,
  PRODUCTION_VALIDATION_RULES,
  DEVELOPMENT_VALIDATION_RULES,
  EnvironmentValidationConfig,
  DEFAULT_VALIDATION_CONFIG
} from '../types/environment.types';

@Injectable({
  providedIn: 'root'
})
export class EnvironmentValidationService {
  private config: EnvironmentValidationConfig = DEFAULT_VALIDATION_CONFIG;

  constructor() {}

  /**
   * Cấu hình validation service
   */
  configure(config: Partial<EnvironmentValidationConfig>): void {
    this.config = { ...this.config, ...config };
  }

  /**
   * Validate toàn bộ environment configuration
   */
  validateEnvironment(environment: Environment): ValidationResult {
    const result: ValidationResult = {
      isValid: true,
      errors: [],
      warnings: []
    };

    try {
      // Basic validation using metadata
      this.validateWithMetadata(environment, ENVIRONMENT_VALIDATION, result);

      // Environment-specific validation
      if (environment.production) {
        this.validateWithMetadata(environment, PRODUCTION_VALIDATION_RULES, result);
      } else {
        this.validateWithMetadata(environment, DEVELOPMENT_VALIDATION_RULES, result);
      }

      // Cross-property validation
      this.validateCrossProperties(environment, result);

      // Performance validation
      this.validatePerformanceSettings(environment, result);

      // Security validation
      this.validateSecuritySettings(environment, result);

    } catch (error) {
      result.errors.push({
        property: 'Environment',
        message: `Unexpected error during validation: ${error}`,
        value: environment
      });
      result.isValid = false;
    }

    return result;
  }

  /**
   * Validate sử dụng validation metadata
   */
  private validateWithMetadata(
    obj: any, 
    metadata: ValidationMetadata, 
    result: ValidationResult,
    prefix: string = ''
  ): void {
    for (const [key, rule] of Object.entries(metadata)) {
      const fullKey = prefix ? `${prefix}.${key}` : key;
      const value = this.getNestedValue(obj, key);
      
      this.validateProperty(fullKey, value, rule, result);
    }
  }

  /**
   * Validate một property với rule
   */
  private validateProperty(
    property: string, 
    value: any, 
    rule: ValidationRule, 
    result: ValidationResult
  ): void {
    // Required validation
    if (rule.required && (value === undefined || value === null || value === '')) {
      result.errors.push({
        property,
        message: `${property} là bắt buộc`,
        value
      });
      result.isValid = false;
      return;
    }

    // Skip other validations if value is empty and not required
    if (!rule.required && (value === undefined || value === null || value === '')) {
      return;
    }

    // String validations
    if (typeof value === 'string') {
      if (rule.minLength && value.length < rule.minLength) {
        result.errors.push({
          property,
          message: `${property} phải có ít nhất ${rule.minLength} ký tự`,
          value
        });
        result.isValid = false;
      }

      if (rule.maxLength && value.length > rule.maxLength) {
        result.errors.push({
          property,
          message: `${property} không được vượt quá ${rule.maxLength} ký tự`,
          value
        });
        result.isValid = false;
      }

      if (rule.pattern && !rule.pattern.test(value)) {
        result.errors.push({
          property,
          message: `${property} không đúng định dạng`,
          value
        });
        result.isValid = false;
      }

      if (rule.url && !this.isValidUrl(value)) {
        result.errors.push({
          property,
          message: `${property} phải là URL hợp lệ`,
          value
        });
        result.isValid = false;
      }

      if (rule.email && !this.isValidEmail(value)) {
        result.errors.push({
          property,
          message: `${property} phải là email hợp lệ`,
          value
        });
        result.isValid = false;
      }
    }

    // Number validations
    if (typeof value === 'number') {
      if (rule.min !== undefined && value < rule.min) {
        result.errors.push({
          property,
          message: `${property} phải >= ${rule.min}`,
          value
        });
        result.isValid = false;
      }

      if (rule.max !== undefined && value > rule.max) {
        result.errors.push({
          property,
          message: `${property} phải <= ${rule.max}`,
          value
        });
        result.isValid = false;
      }
    }

    // Custom validation
    if (rule.custom) {
      const customError = rule.custom(value);
      if (customError) {
        result.warnings.push({
          property,
          message: customError,
          value
        });
      }
    }
  }

  /**
   * Validate cross-property dependencies
   */
  private validateCrossProperties(environment: Environment, result: ValidationResult): void {
    // Pagination consistency
    if (environment.pagination.defaultPageSize > environment.pagination.maxPageSize) {
      result.errors.push({
        property: 'pagination.defaultPageSize',
        message: 'defaultPageSize không được lớn hơn maxPageSize',
        value: environment.pagination.defaultPageSize
      });
      result.isValid = false;
    }

    // API URL và endpoints consistency
    if (environment.apiUrl.endsWith('/') && 
        (environment.apiEndpoints.products.startsWith('/') || 
         environment.apiEndpoints.categories.startsWith('/'))) {
      result.warnings.push({
        property: 'apiUrl',
        message: 'apiUrl kết thúc bằng "/" và endpoints bắt đầu bằng "/" có thể gây double slash',
        value: environment.apiUrl
      });
    }
  }

  /**
   * Validate performance settings
   */
  private validatePerformanceSettings(environment: Environment, result: ValidationResult): void {
    // HTTP timeout warnings
    if (environment.httpTimeout > 60000) {
      result.warnings.push({
        property: 'httpTimeout',
        message: 'HTTP timeout > 60 giây có thể gây trải nghiệm người dùng kém',
        value: environment.httpTimeout
      });
    }

    if (environment.httpTimeout < 5000) {
      result.warnings.push({
        property: 'httpTimeout',
        message: 'HTTP timeout < 5 giây có thể gây timeout cho các request chậm',
        value: environment.httpTimeout
      });
    }

    // Pagination performance
    if (environment.pagination.defaultPageSize > 50) {
      result.warnings.push({
        property: 'pagination.defaultPageSize',
        message: 'defaultPageSize > 50 có thể ảnh hưởng đến performance',
        value: environment.pagination.defaultPageSize
      });
    }
  }

  /**
   * Validate security settings
   */
  private validateSecuritySettings(environment: Environment, result: ValidationResult): void {
    // Production security checks
    if (environment.production) {
      if (environment.apiUrl.startsWith('http://')) {
        result.errors.push({
          property: 'apiUrl',
          message: 'Production environment phải sử dụng HTTPS',
          value: environment.apiUrl
        });
        result.isValid = false;
      }

      if (environment.enableDebugLogging) {
        result.warnings.push({
          property: 'enableDebugLogging',
          message: 'Production environment không nên enable debug logging',
          value: environment.enableDebugLogging
        });
      }
    }
  }

  /**
   * Get nested object value by dot notation
   */
  private getNestedValue(obj: any, path: string): any {
    return path.split('.').reduce((current, key) => current?.[key], obj);
  }

  /**
   * Validate URL format
   */
  private isValidUrl(url: string): boolean {
    try {
      new URL(url);
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Validate email format
   */
  private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  /**
   * Format validation result for display
   */
  formatValidationResult(result: ValidationResult): string {
    let message = '';

    if (result.errors.length > 0) {
      message += 'ERRORS:\n';
      result.errors.forEach(error => {
        message += `- ${error.property}: ${error.message}\n`;
      });
      message += '\n';
    }

    if (result.warnings.length > 0) {
      message += 'WARNINGS:\n';
      result.warnings.forEach(warning => {
        message += `- ${warning.property}: ${warning.message}\n`;
      });
    }

    return message;
  }
}
