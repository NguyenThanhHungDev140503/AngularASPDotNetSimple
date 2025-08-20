# ProductAPI - Hướng Dẫn Nhanh

## Bắt Đầu Trong 5 Phút

### 1. Khởi Chạy API
```bash
cd ProductAPI
dotnet run
```

API sẽ chạy tại: `http://localhost:5046`
Swagger UI: `http://localhost:5046`

### 2. Tạo Danh Mục Đầu Tiên

**POST** `http://localhost:5046/api/categories`
```json
{
  "name": "Giày thể thao",
  "description": "Danh mục giày thể thao cao cấp"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Thành công",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Giày thể thao",
    "description": "Danh mục giày thể thao cao cấp",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  }
}
```

### 3. Tạo Sản Phẩm Đầu Tiên

**POST** `http://localhost:5046/api/products`
```json
{
  "name": "Nike Air Max 270",
  "description": "Giày thể thao Nike Air Max 270 chính hãng",
  "stockPrice": 1500000,
  "price": 2200000,
  "stockQuantity": 20,
  "categoryId": "550e8400-e29b-41d4-a716-446655440000"
}
```

### 4. Lấy Danh Sách Sản Phẩm

**GET** `http://localhost:5046/api/products`

### 5. Tìm Kiếm Sản Phẩm

**GET** `http://localhost:5046/api/products?searchTerm=Nike&page=1&pageSize=10`

## Các Endpoint Chính

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/products` | Lấy danh sách sản phẩm (có phân trang) |
| GET | `/api/products/{id}` | Lấy sản phẩm theo ID |
| POST | `/api/products` | Tạo sản phẩm mới |
| PUT | `/api/products/{id}` | Cập nhật sản phẩm |
| DELETE | `/api/products/{id}` | Xóa sản phẩm |
| GET | `/api/categories` | Lấy danh sách danh mục |
| POST | `/api/categories` | Tạo danh mục mới |
| PUT | `/api/categories/{id}` | Cập nhật danh mục |
| DELETE | `/api/categories/{id}` | Xóa danh mục |

## Cấu Trúc Response

Tất cả API đều trả về theo format:
```json
{
  "success": true/false,
  "message": "Thông báo",
  "data": { /* Dữ liệu */ },
  "errors": [ /* Danh sách lỗi nếu có */ ]
}
```

## Validation Rules

### Product
- `name`: Bắt buộc, tối đa 200 ký tự
- `stockPrice`: Bắt buộc, >= 0
- `price`: Bắt buộc, >= 0
- `stockQuantity`: Bắt buộc, >= 0
- `categoryId`: Tùy chọn

### Category
- `name`: Bắt buộc, tối đa 100 ký tự
- `description`: Tùy chọn

## Query Parameters cho GET /api/products

| Parameter | Type | Default | Mô tả |
|-----------|------|---------|-------|
| `page` | int | 1 | Số trang |
| `pageSize` | int | 10 | Số items per page |
| `searchTerm` | string | - | Tìm kiếm theo tên |
| `categoryId` | Guid | - | Lọc theo danh mục |
| `minPrice` | decimal | - | Giá tối thiểu |
| `maxPrice` | decimal | - | Giá tối đa |
| `sortBy` | string | "Name" | Sắp xếp theo field |
| `sortOrder` | string | "asc" | Thứ tự (asc/desc) |

## Ví Dụ Curl Commands

### Tạo danh mục
```bash
curl -X POST "http://localhost:5046/api/categories" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Giày cao gót",
    "description": "Danh mục giày cao gót thời trang"
  }'
```

### Tạo sản phẩm
```bash
curl -X POST "http://localhost:5046/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Giày cao gót Gucci",
    "description": "Giày cao gót Gucci chính hãng",
    "stockPrice": 8000000,
    "price": 12000000,
    "stockQuantity": 5,
    "categoryId": "CATEGORY_ID_HERE"
  }'
```

### Lấy sản phẩm với filter
```bash
curl "http://localhost:5046/api/products?searchTerm=Gucci&minPrice=5000000&maxPrice=15000000&sortBy=Price&sortOrder=desc"
```

### Cập nhật sản phẩm
```bash
curl -X PUT "http://localhost:5046/api/products/PRODUCT_ID_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Giày cao gót Gucci - Updated",
    "description": "Mô tả đã cập nhật",
    "stockPrice": 8500000,
    "price": 12500000,
    "stockQuantity": 3,
    "categoryId": "CATEGORY_ID_HERE"
  }'
```

### Xóa sản phẩm
```bash
curl -X DELETE "http://localhost:5046/api/products/PRODUCT_ID_HERE"
```

## Status Codes

| Code | Meaning | Khi nào xảy ra |
|------|---------|----------------|
| 200 | OK | Request thành công |
| 201 | Created | Tạo resource thành công |
| 400 | Bad Request | Dữ liệu không hợp lệ |
| 404 | Not Found | Không tìm thấy resource |
| 500 | Internal Server Error | Lỗi server |

## Postman Collection

Import file `ProductAPI.postman_collection.json` vào Postman để có sẵn tất cả các request mẫu.

## Troubleshooting Nhanh

### API không khởi động được
```bash
# Kiểm tra port
netstat -ano | findstr :5046

# Chạy với port khác
dotnet run --urls "http://localhost:5047"
```

### Database connection error
```bash
# Update database
dotnet ef database update

# Hoặc reset database
dotnet ef database drop
dotnet ef database update
```

### CORS error từ browser
- API đã cấu hình CORS cho tất cả origins
- Kiểm tra browser console để xem chi tiết lỗi
- Thử với Postman hoặc curl để loại trừ CORS issue

## Liên Kết Hữu Ích

- **Swagger UI**: `http://localhost:5046`
- **API Base URL**: `http://localhost:5046/api`
- **Health Check**: `http://localhost:5046/health` (nếu có)
- **Tài liệu đầy đủ**: `ProductAPI_Documentation.md`

## Workflow Cơ Bản

1. **Tạo danh mục** → Lưu `categoryId`
2. **Tạo sản phẩm** với `categoryId` từ bước 1
3. **Tìm kiếm/lọc** sản phẩm theo nhu cầu
4. **Cập nhật** thông tin khi cần
5. **Xóa** khi không còn sử dụng

---

*Để biết thêm chi tiết, xem file `ProductAPI_Documentation.md`*
