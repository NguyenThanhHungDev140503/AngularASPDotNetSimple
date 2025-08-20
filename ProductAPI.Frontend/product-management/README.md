# ProductAPI Frontend

Ứng dụng Angular Frontend đơn giản để tương tác với ProductAPI Backend.

## Cấu trúc dự án

```
ProductAPI.Frontend/
└── product-management/          # Angular application
    ├── src/
    │   └── app/
    │       ├── product/         # Product module
    │       │   ├── components/
    │       │   │   └── product-list/  # Product list component
    │       │   ├── models/      # TypeScript interfaces
    │       │   ├── services/    # HTTP services
    │       │   ├── product-module.ts
    │       │   └── product-routing-module.ts
    │       ├── app.config.ts    # App configuration
    │       ├── app.routes.ts    # Main routing
    │       └── app.html         # Main template
    └── package.json
```

## Công nghệ sử dụng

- **Angular 18+** (Zoneless)
- **TypeScript**
- **RxJS** cho HTTP calls
- **CSS thuần** cho styling
- **HttpClientModule** cho API calls

## Tính năng

- ✅ Hiển thị danh sách sản phẩm từ API
- ✅ Phân trang (pagination)
- ✅ Loading states
- ✅ Error handling


## Cài đặt và chạy

### Yêu cầu
- Node.js 18+
- Angular CLI

### Bước 1: Cài đặt dependencies
```bash
cd ProductAPI.Frontend/product-management
npm install
```

### Bước 2: Chạy development server
```bash
ng serve
```

Ứng dụng sẽ chạy tại: http://localhost:4200/

### Bước 3: Đảm bảo Backend API đang chạy
Backend API cần chạy tại: http://localhost:5046/

## API Integration

Frontend gọi các endpoints sau:
- `GET /api/products` - Lấy danh sách sản phẩm với phân trang

### Cấu hình API URL
Trong file `src/app/product/services/product.ts`:
```typescript
private readonly apiUrl = 'http://localhost:5046/api/products';
```

## Cấu trúc Components

### ProductListComponent
- **File**: `src/app/product/components/product-list/`
- **Chức năng**: Hiển thị danh sách sản phẩm với phân trang
- **Features**:
  - Loading indicator
  - Error handling
  - Pagination controls
  - Responsive table

### ProductService
- **File**: `src/app/product/services/product.ts`
- **Chức năng**: HTTP service để gọi API
- **Methods**:
  - `getProducts(query: ProductQuery)`: Lấy danh sách sản phẩm

## Models

### Product Interface
```typescript
interface Product {
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
```

## Build Production

```bash
ng build --configuration production
```

Files sẽ được tạo trong thư mục `dist/`.

## Lưu ý

- Đây là ví dụ cơ bản cho người mới học Angular
- Chỉ có chức năng đọc dữ liệu (Read-only)
- Không có authentication
- Sử dụng CSS thuần thay vì framework CSS

## Mở rộng

Có thể thêm các tính năng:
- CRUD operations (Create, Update, Delete)
- Search và filtering
- Authentication
- Form validation
- Unit tests
- E2E tests
