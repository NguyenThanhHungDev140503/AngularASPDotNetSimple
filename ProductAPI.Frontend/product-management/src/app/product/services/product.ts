import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, ProductQuery, PagedResult, ApiResponse } from '../models/product.model';

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

    // API already returns lowercase properties, so no mapping needed
    return this.http.get<ApiResponse<PagedResult<Product>>>(this.apiUrl, { params });
  }
}
