/**
 * Environment configuration types với validation metadata
 */

export interface ValidationRule {
  required?: boolean;
  minLength?: number;
  maxLength?: number;
  min?: number;
  max?: number;
  pattern?: RegExp;
  url?: boolean;
  email?: boolean;
  custom?: (value: any) => string | null;
}

export interface ValidationMetadata {
  [key: string]: ValidationRule;
}

export interface ApiEndpoints {
  products: string;
  categories: string;
}

export interface PaginationConfig {
  defaultPageSize: number;
  maxPageSize: number;
}

export interface Environment {
  production: boolean;
  apiUrl: string;
  apiEndpoints: ApiEndpoints;
  pagination: PaginationConfig;
  httpTimeout: number;
  enableDebugLogging: boolean;
  appName: string;
  version: string;
}

/**
 * Validation metadata cho environment configuration
 */
export const ENVIRONMENT_VALIDATION: ValidationMetadata = {
  'production': {
    required: true
  },
  'apiUrl': {
    required: true,
    url: true,
    minLength: 10
  },
  'apiEndpoints.products': {
    required: true,
    minLength: 1,
    pattern: /^\/[a-zA-Z0-9\-_\/]*$/
  },
  'apiEndpoints.categories': {
    required: true,
    minLength: 1,
    pattern: /^\/[a-zA-Z0-9\-_\/]*$/
  },
  'pagination.defaultPageSize': {
    required: true,
    min: 1,
    max: 100
  },
  'pagination.maxPageSize': {
    required: true,
    min: 10,
    max: 1000
  },
  'httpTimeout': {
    required: true,
    min: 1000,
    max: 300000
  },
  'enableDebugLogging': {
    required: true
  },
  'appName': {
    required: true,
    minLength: 3,
    maxLength: 100
  },
  'version': {
    required: true,
    pattern: /^\d+\.\d+\.\d+$/
  }
};

/**
 * Environment-specific validation rules
 */
export const PRODUCTION_VALIDATION_RULES: ValidationMetadata = {
  'apiUrl': {
    custom: (value: string) => {
      if (!value.startsWith('https://')) {
        return 'Production environment phải sử dụng HTTPS';
      }
      return null;
    }
  },
  'enableDebugLogging': {
    custom: (value: boolean) => {
      if (value === true) {
        return 'Production environment không nên enable debug logging';
      }
      return null;
    }
  }
};

export const DEVELOPMENT_VALIDATION_RULES: ValidationMetadata = {
  'apiUrl': {
    custom: (value: string) => {
      if (!value.startsWith('http://localhost') && !value.startsWith('https://localhost')) {
        return 'Development environment nên sử dụng localhost';
      }
      return null;
    }
  }
};

/**
 * Validation result types
 */
export interface ValidationError {
  property: string;
  message: string;
  value?: any;
}

export interface ValidationWarning {
  property: string;
  message: string;
  value?: any;
}

export interface ValidationResult {
  isValid: boolean;
  errors: ValidationError[];
  warnings: ValidationWarning[];
}

/**
 * Environment validation configuration
 */
export interface EnvironmentValidationConfig {
  validateOnStartup: boolean;
  throwOnError: boolean;
  logWarnings: boolean;
  enableRuntimeValidation: boolean;
}

export const DEFAULT_VALIDATION_CONFIG: EnvironmentValidationConfig = {
  validateOnStartup: true,
  throwOnError: true,
  logWarnings: true,
  enableRuntimeValidation: false
};
