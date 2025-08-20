# ProductAPI

RESTful Web API được xây dựng bằng ASP.NET Core 9.0 và Entity Framework Core để quản lý sản phẩm và danh mục.

## Công Nghệ

- ASP.NET Core 9.0
- Entity Framework Core 9.0
- SQL Server
- Swagger/OpenAPI
- AutoMapper

## Khởi Chạy Nhanh

### Yêu Cầu
- .NET 9.0 SDK
- SQL Server

### Cài Đặt
```bash
# setup
cd ProductAPI

# Restore packages
dotnet restore

# Tạo database
## Trong SQL Server
- Chạy "sql\theshoe_mssql_create_db.sql" để tạo database
- Chạy "sql\add_data.sql" để thêm data
- chỉnh sửa database connection string trong "ProductAPI\appsettings.json"
## Migrations
dotnet ef database update

# Chạy API
dotnet run
```

**API URL**: `http://localhost:5046`
**Swagger UI**: `http://localhost:5046`

### Chạy Tests
```bash
dotnet test
```

## API Endpoints

### Products
- `GET /api/products` - Danh sách sản phẩm (có phân trang, tìm kiếm, lọc)
- `GET /api/products/{id}` - Chi tiết sản phẩm
- `POST /api/products` - Tạo sản phẩm
- `PUT /api/products/{id}` - Cập nhật sản phẩm
- `DELETE /api/products/{id}` - Xóa sản phẩm

### Categories
- `GET /api/categories` - Danh sách danh mục
- `POST /api/categories` - Tạo danh mục
- `PUT /api/categories/{id}` - Cập nhật danh mục
- `DELETE /api/categories/{id}` - Xóa danh mục

## Tài Liệu

- **Tài liệu đầy đủ**: `ProductAPI_Documentation.md`
- **Hướng dẫn nhanh**: `ProductAPI_QuickStart.md`

## Cấu Trúc Dự Án

```
ProductAPI/
├── Controllers/     # API Controllers
├── Models/         # Entities & DTOs
├── Services/       # Business Logic
├── Repositories/   # Data Access
├── Data/          # DbContext
└── Tests/         # Unit Tests
```
