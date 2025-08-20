export interface Category {
  id: string;
  name: string;
  description?: string;
}

export interface Product {
  id: string;
  name: string;
  description?: string;
  stockPrice: number;
  price: number;
  stockQuantity: number;
  categoryId?: string;
  category?: Category;
  createdAt: string;
  updatedAt: string;
}

export interface ProductQuery {
  page: number;
  pageSize: number;
  searchTerm?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: string;
  sortOrder?: string;
}

export interface PagedResult<T> {
  items: T[];  // Keep lowercase for frontend consistency
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// API Response interface that matches C# backend exactly
export interface ApiPagedResult<T> {
  Items: T[];  // Uppercase to match C# backend
  TotalCount: number;
  Page: number;
  PageSize: number;
  TotalPages: number;
  HasNextPage: boolean;
  HasPreviousPage: boolean;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}
