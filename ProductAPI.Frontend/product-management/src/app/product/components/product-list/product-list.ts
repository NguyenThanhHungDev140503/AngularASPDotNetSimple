import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ProductService } from '../../services/product';
import { Product, ProductQuery, PagedResult, ApiResponse } from '../../models/product.model';
import { environment } from '../../../../environments/environment';

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
  pageSize = environment.pagination.defaultPageSize;
  totalCount = 0;
  totalPages = 0;
  hasNextPage = false;
  hasPreviousPage = false;

  constructor(
    private productService: ProductService,
    private cdr: ChangeDetectorRef
  ) { }

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
        if (environment.enableDebugLogging) {
          console.log('API Response:', response); // Debug logging
          console.log('Setting loading to false...'); // Debug logging
        }
        this.loading = false;
        if (environment.enableDebugLogging) {
          console.log('Loading is now:', this.loading); // Debug logging
        }

        if (response.success && response.data) {
          if (environment.enableDebugLogging) {
            console.log('Response data:', response.data); // Debug logging
            console.log('Items array:', response.data.items); // Debug logging
          }

          this.products = response.data.items || [];
          this.totalCount = response.data.totalCount || 0;
          this.totalPages = response.data.totalPages || 0;
          this.hasNextPage = response.data.hasNextPage || false;
          this.hasPreviousPage = response.data.hasPreviousPage || false;

          if (environment.enableDebugLogging) {
            console.log('Products loaded:', this.products.length); // Debug logging
            console.log('Products array:', this.products); // Debug logging
            console.log('Component state - loading:', this.loading, 'error:', this.error); // Debug logging
          }

          // Force change detection
          this.cdr.detectChanges();

          if (environment.enableDebugLogging) {
            console.log('Change detection triggered'); // Debug logging
          }

          if (this.products.length === 0 && environment.enableDebugLogging) {
            console.warn('No products found in response');
          }
        } else {
          if (environment.enableDebugLogging) {
            console.error('API response failed:', response);
          }
          this.error = response.message || 'Có lỗi xảy ra khi tải dữ liệu';
          this.cdr.detectChanges(); // Force change detection for error case too
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = 'Không thể kết nối đến server. Vui lòng kiểm tra lại.';
        this.cdr.detectChanges(); // Force change detection for error case
        if (environment.enableDebugLogging) {
          console.error('Error loading products:', err);
        }
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
