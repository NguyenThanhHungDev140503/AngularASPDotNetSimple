import { Environment } from '../app/core/types/environment.types';

export const environment: Environment = {
  production: true,
  apiUrl: 'https://api.productmanagement.com/api', // Production API URL
  apiEndpoints: {
    products: '/products',
    categories: '/categories'
  },
  pagination: {
    defaultPageSize: 20,
    maxPageSize: 100
  },
  httpTimeout: 60000, // 60 seconds for production
  enableDebugLogging: false,
  appName: 'Product Management System',
  version: '1.0.0'
};
