import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Product, ProductQuery, PagedResult, ApiResponse, ApiPagedResult } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly apiUrl = 'http://localhost:5046/api/products';

  constructor(private http: HttpClient) { }

  getProducts(query: ProductQuery): Observable<ApiResponse<PagedResult<Product>>> {
    let params = new HttpParams()
      .set('page', query.page.toString())
      .set('pageSize', query.pageSize.toString());

    if (query.searchTerm) {
      params = params.set('searchTerm', query.searchTerm);
    }
    if (query.categoryId) {
      params = params.set('categoryId', query.categoryId);
    }
    if (query.minPrice !== undefined) {
      params = params.set('minPrice', query.minPrice.toString());
    }
    if (query.maxPrice !== undefined) {
      params = params.set('maxPrice', query.maxPrice.toString());
    }
    if (query.sortBy) {
      params = params.set('sortBy', query.sortBy);
    }
    if (query.sortOrder) {
      params = params.set('sortOrder', query.sortOrder);
    }

    // Get raw response from API with uppercase properties
    return this.http.get<ApiResponse<ApiPagedResult<Product>>>(this.apiUrl, { params })
      .pipe(
        map(response => {
          console.log('Raw API Response:', response); // Debug log
          console.log('Response data before mapping:', response.data); // Debug log

          // Transform API response to match frontend interface
          if (response.success && response.data) {
            console.log('Mapping data...'); // Debug log
            console.log('Original Items:', response.data.Items); // Debug log
            console.log('Original TotalCount:', response.data.TotalCount); // Debug log

            const transformedData: PagedResult<Product> = {
              items: response.data.Items,  // Map uppercase Items to lowercase items
              totalCount: response.data.TotalCount,
              page: response.data.Page,
              pageSize: response.data.PageSize,
              totalPages: response.data.TotalPages,
              hasNextPage: response.data.HasNextPage,
              hasPreviousPage: response.data.HasPreviousPage
            };

            console.log('Transformed data:', transformedData); // Debug log

            return {
              success: response.success,
              message: response.message,
              data: transformedData,
              errors: response.errors
            } as ApiResponse<PagedResult<Product>>;
          }

          console.log('Response not successful or no data'); // Debug log
          // For error responses, return without data transformation
          return {
            success: response.success,
            message: response.message,
            data: undefined,
            errors: response.errors
          } as ApiResponse<PagedResult<Product>>;
        })
      );
  }
}
