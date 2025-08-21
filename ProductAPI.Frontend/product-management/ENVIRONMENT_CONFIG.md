# Environment Configuration Setup

## Tổng quan
Đã cấu hình thành công environment variables cho ứng dụng Angular Frontend để quản lý cấu hình tập trung và có thể thay đổi theo từng môi trường triển khai.

## Các file đã tạo/cập nhật

### 1. Environment Files
- `src/environments/environment.ts` - Cấu hình cho development
- `src/environments/environment.prod.ts` - Cấu hình cho production

### 2. Cấu hình Angular
- `angular.json` - Đã thêm fileReplacements cho production build

### 3. Services đã cập nhật
- `src/app/product/services/product.ts` - Sử dụng environment.apiUrl thay vì hard-coded URL
- `src/app/product/services/http-config.service.ts` - Service mới để quản lý HTTP timeout

### 4. Components đã cập nhật
- `src/app/product/components/product-list/product-list.ts` - Sử dụng environment cho pagination và debug logging

## Cấu hình Environment

### Development (environment.ts)
```typescript
{
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
}
```

### Production (environment.prod.ts)
```typescript
{
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
}
```

## Lợi ích đạt được

1. **Quản lý cấu hình tập trung**: Tất cả cấu hình được tập trung trong environment files
2. **Dễ bảo trì**: Thay đổi cấu hình không cần sửa code
3. **Môi trường riêng biệt**: Cấu hình khác nhau cho development và production
4. **Bảo mật**: Production không hiển thị debug logs
5. **Linh hoạt**: Dễ dàng thêm cấu hình mới

## Cách sử dụng

### Build cho Development
```bash
npm run build
# hoặc
ng build --configuration development
```

### Build cho Production
```bash
npm run build -- --configuration production
# hoặc
ng build --configuration production
```

### Serve cho Development
```bash
npm start
# hoặc
ng serve
```

## Kiểm tra thành công
- ✅ Development build thành công
- ✅ Production build thành công
- ✅ Environment files được load đúng
- ✅ API URL được cấu hình từ environment
- ✅ Pagination settings từ environment
- ✅ Debug logging được điều khiển bởi environment
