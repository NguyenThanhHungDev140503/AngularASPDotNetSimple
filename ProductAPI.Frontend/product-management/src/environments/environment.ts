// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { Environment } from '../app/core/types/environment.types';

export const environment: Environment = {
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
  httpTimeout: 30000, // 30 seconds
  enableDebugLogging: true,
  appName: 'Product Management System',
  version: '1.0.0'
};
