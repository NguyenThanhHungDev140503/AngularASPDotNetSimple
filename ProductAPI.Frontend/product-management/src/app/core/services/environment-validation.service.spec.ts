import { TestBed } from '@angular/core/testing';
import { EnvironmentValidationService } from './environment-validation.service';
import { Environment } from '../types/environment.types';

describe('EnvironmentValidationService', () => {
  let service: EnvironmentValidationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EnvironmentValidationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('validateEnvironment', () => {
    it('should validate a valid development environment', () => {
      // Arrange
      const validEnvironment: Environment = {
        production: false,
        apiUrl: 'http://localhost:5046/api',
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 10,
          maxPageSize: 100
        },
        httpTimeout: 30000,
        enableDebugLogging: true,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(validEnvironment);

      // Assert
      expect(result.isValid).toBe(true);
      expect(result.errors.length).toBe(0);
    });

    it('should validate a valid production environment', () => {
      // Arrange
      const validEnvironment: Environment = {
        production: true,
        apiUrl: 'https://api.productmanagement.com/api',
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 20,
          maxPageSize: 100
        },
        httpTimeout: 60000,
        enableDebugLogging: false,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(validEnvironment);

      // Assert
      expect(result.isValid).toBe(true);
      expect(result.errors.length).toBe(0);
    });

    it('should return error for missing required fields', () => {
      // Arrange
      const invalidEnvironment = {
        production: false,
        // apiUrl missing
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 10,
          maxPageSize: 100
        },
        httpTimeout: 30000,
        enableDebugLogging: true,
        // appName missing
        version: '1.0.0'
      } as Environment;

      // Act
      const result = service.validateEnvironment(invalidEnvironment);

      // Assert
      expect(result.isValid).toBe(false);
      expect(result.errors.some(e => e.property === 'apiUrl')).toBe(true);
      expect(result.errors.some(e => e.property === 'appName')).toBe(true);
    });

    it('should return error for invalid URL format', () => {
      // Arrange
      const invalidEnvironment: Environment = {
        production: false,
        apiUrl: 'invalid-url', // Invalid URL
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 10,
          maxPageSize: 100
        },
        httpTimeout: 30000,
        enableDebugLogging: true,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(invalidEnvironment);

      // Assert
      expect(result.isValid).toBe(false);
      expect(result.errors.some(e => e.property === 'apiUrl')).toBe(true);
    });

    it('should return error for invalid version format', () => {
      // Arrange
      const invalidEnvironment: Environment = {
        production: false,
        apiUrl: 'http://localhost:5046/api',
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 10,
          maxPageSize: 100
        },
        httpTimeout: 30000,
        enableDebugLogging: true,
        appName: 'Product Management System',
        version: 'invalid-version' // Invalid version format
      };

      // Act
      const result = service.validateEnvironment(invalidEnvironment);

      // Assert
      expect(result.isValid).toBe(false);
      expect(result.errors.some(e => e.property === 'version')).toBe(true);
    });

    it('should return error for invalid endpoint format', () => {
      // Arrange
      const invalidEnvironment: Environment = {
        production: false,
        apiUrl: 'http://localhost:5046/api',
        apiEndpoints: {
          products: 'products', // Missing leading slash
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 10,
          maxPageSize: 100
        },
        httpTimeout: 30000,
        enableDebugLogging: true,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(invalidEnvironment);

      // Assert
      expect(result.isValid).toBe(false);
      expect(result.errors.some(e => e.property === 'apiEndpoints.products')).toBe(true);
    });

    it('should return error for invalid pagination values', () => {
      // Arrange
      const invalidEnvironment: Environment = {
        production: false,
        apiUrl: 'http://localhost:5046/api',
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 0, // Invalid: too small
          maxPageSize: 2000 // Invalid: too large
        },
        httpTimeout: 30000,
        enableDebugLogging: true,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(invalidEnvironment);

      // Assert
      expect(result.isValid).toBe(false);
      expect(result.errors.some(e => e.property === 'pagination.defaultPageSize')).toBe(true);
      expect(result.errors.some(e => e.property === 'pagination.maxPageSize')).toBe(true);
    });

    it('should return error for invalid timeout values', () => {
      // Arrange
      const invalidEnvironment: Environment = {
        production: false,
        apiUrl: 'http://localhost:5046/api',
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 10,
          maxPageSize: 100
        },
        httpTimeout: 500, // Invalid: too small
        enableDebugLogging: true,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(invalidEnvironment);

      // Assert
      expect(result.isValid).toBe(false);
      expect(result.errors.some(e => e.property === 'httpTimeout')).toBe(true);
    });

    it('should return error when defaultPageSize > maxPageSize', () => {
      // Arrange
      const invalidEnvironment: Environment = {
        production: false,
        apiUrl: 'http://localhost:5046/api',
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 50, // Greater than maxPageSize
          maxPageSize: 30
        },
        httpTimeout: 30000,
        enableDebugLogging: true,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(invalidEnvironment);

      // Assert
      expect(result.isValid).toBe(false);
      expect(result.errors.some(e => e.property === 'pagination.defaultPageSize')).toBe(true);
    });

    it('should return error for production with HTTP URL', () => {
      // Arrange
      const invalidEnvironment: Environment = {
        production: true, // Production
        apiUrl: 'http://api.productmanagement.com/api', // HTTP instead of HTTPS
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 20,
          maxPageSize: 100
        },
        httpTimeout: 60000,
        enableDebugLogging: false,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(invalidEnvironment);

      // Assert
      expect(result.isValid).toBe(false);
      expect(result.errors.some(e => e.property === 'apiUrl')).toBe(true);
    });

    it('should return warning for production with debug logging enabled', () => {
      // Arrange
      const environment: Environment = {
        production: true,
        apiUrl: 'https://api.productmanagement.com/api',
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 20,
          maxPageSize: 100
        },
        httpTimeout: 60000,
        enableDebugLogging: true, // Should be false in production
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(environment);

      // Assert
      expect(result.isValid).toBe(true); // Valid but with warnings
      expect(result.warnings.some(w => w.property === 'enableDebugLogging')).toBe(true);
    });

    it('should return warning for high timeout values', () => {
      // Arrange
      const environment: Environment = {
        production: false,
        apiUrl: 'http://localhost:5046/api',
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 10,
          maxPageSize: 100
        },
        httpTimeout: 120000, // Very high timeout
        enableDebugLogging: true,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(environment);

      // Assert
      expect(result.isValid).toBe(true);
      expect(result.warnings.some(w => w.property === 'httpTimeout')).toBe(true);
    });

    it('should return warning for high default page size', () => {
      // Arrange
      const environment: Environment = {
        production: false,
        apiUrl: 'http://localhost:5046/api',
        apiEndpoints: {
          products: '/products',
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 60, // High page size
          maxPageSize: 100
        },
        httpTimeout: 30000,
        enableDebugLogging: true,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(environment);

      // Assert
      expect(result.isValid).toBe(true);
      expect(result.warnings.some(w => w.property === 'pagination.defaultPageSize')).toBe(true);
    });

    it('should return warning for URL ending with slash and endpoints starting with slash', () => {
      // Arrange
      const environment: Environment = {
        production: false,
        apiUrl: 'http://localhost:5046/api/', // Ends with slash
        apiEndpoints: {
          products: '/products', // Starts with slash
          categories: '/categories'
        },
        pagination: {
          defaultPageSize: 10,
          maxPageSize: 100
        },
        httpTimeout: 30000,
        enableDebugLogging: true,
        appName: 'Product Management System',
        version: '1.0.0'
      };

      // Act
      const result = service.validateEnvironment(environment);

      // Assert
      expect(result.isValid).toBe(true);
      expect(result.warnings.some(w => w.property === 'apiUrl')).toBe(true);
    });
  });

  describe('formatValidationResult', () => {
    it('should format validation result with errors and warnings', () => {
      // Arrange
      const result = {
        isValid: false,
        errors: [
          { property: 'apiUrl', message: 'API URL is required', value: null }
        ],
        warnings: [
          { property: 'httpTimeout', message: 'Timeout is very high', value: 120000 }
        ]
      };

      // Act
      const formatted = service.formatValidationResult(result);

      // Assert
      expect(formatted).toContain('ERRORS:');
      expect(formatted).toContain('apiUrl: API URL is required');
      expect(formatted).toContain('WARNINGS:');
      expect(formatted).toContain('httpTimeout: Timeout is very high');
    });

    it('should format validation result with only errors', () => {
      // Arrange
      const result = {
        isValid: false,
        errors: [
          { property: 'version', message: 'Invalid version format', value: 'invalid' }
        ],
        warnings: []
      };

      // Act
      const formatted = service.formatValidationResult(result);

      // Assert
      expect(formatted).toContain('ERRORS:');
      expect(formatted).toContain('version: Invalid version format');
      expect(formatted).not.toContain('WARNINGS:');
    });

    it('should format validation result with only warnings', () => {
      // Arrange
      const result = {
        isValid: true,
        errors: [],
        warnings: [
          { property: 'enableDebugLogging', message: 'Debug logging in production', value: true }
        ]
      };

      // Act
      const formatted = service.formatValidationResult(result);

      // Assert
      expect(formatted).not.toContain('ERRORS:');
      expect(formatted).toContain('WARNINGS:');
      expect(formatted).toContain('enableDebugLogging: Debug logging in production');
    });
  });
});
