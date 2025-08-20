import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product';
import { Product, ProductQuery, PagedResult, ApiResponse } from '../../models/product.model';

@Component({
  selector: 'app-product-list',
  standalone: false,
  templateUrl: './product-list.html',
  styleUrl: './product-list.css'
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  loading = false;
  error: string | null = null;

  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;
  hasNextPage = false;
  hasPreviousPage = false;

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.error = null;

    const query: ProductQuery = {
      page: this.currentPage,
      pageSize: this.pageSize,
      sortBy: 'Name',
      sortOrder: 'asc'
    };

    this.productService.getProducts(query).subscribe({
      next: (response: ApiResponse<PagedResult<Product>>) => {
        console.log('API Response:', response); // Debug logging
        this.loading = false;

        if (response.success && response.data) {
          console.log('Response data:', response.data); // Debug logging
          console.log('Items array:', response.data.items); // Debug logging

          this.products = response.data.items || [];
          this.totalCount = response.data.totalCount || 0;
          this.totalPages = response.data.totalPages || 0;
          this.hasNextPage = response.data.hasNextPage || false;
          this.hasPreviousPage = response.data.hasPreviousPage || false;

          console.log('Products loaded:', this.products.length); // Debug logging

          if (this.products.length === 0) {
            console.warn('No products found in response');
          }
        } else {
          console.error('API response failed:', response);
          this.error = response.message || 'Có lỗi xảy ra khi tải dữ liệu';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = 'Không thể kết nối đến server. Vui lòng kiểm tra lại.';
        console.error('Error loading products:', err);
      }
    });
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadProducts();
    }
  }

  nextPage(): void {
    if (this.hasNextPage) {
      this.goToPage(this.currentPage + 1);
    }
  }

  previousPage(): void {
    if (this.hasPreviousPage) {
      this.goToPage(this.currentPage - 1);
    }
  }

  // TrackBy function for better performance
  trackByProductId(index: number, product: Product): string {
    return product.id;
  }
}
