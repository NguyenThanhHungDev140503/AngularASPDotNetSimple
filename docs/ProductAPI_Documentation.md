# Tài Liệu API - ProductAPI

## Tổng Quan

ProductAPI là một RESTful Web API được xây dựng bằng ASP.NET Core 9.0, cung cấp các chức năng quản lý sản phẩm và danh mục sản phẩm với đầy đủ các thao tác CRUD (Create, Read, Update, Delete).

## Thông Tin Kỹ Thuật

### Công Nghệ Sử Dụng
- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server Express (TheShoe database)
- **ORM**: Entity Framework Core 9.0.8
- **Documentation**: Swagger/OpenAPI 3.0
- **Mapping**: AutoMapper 12.0.1
- **Testing**: xUnit với Entity Framework InMemory

### Cấu Hình Môi Trường
- **Base URL**: `http://localhost:5046`
- **Swagger UI**: `http://localhost:5046` (Development mode)
- **Database Connection**: SQL Server Express với Trusted Connection
- **CORS**: Cho phép tất cả origins, methods và headers

### Cấu Trúc Dự Án
```
ProductAPI/
├── Controllers/           # API Controllers
├── Models/
│   ├── Entities/         # Database entities
│   ├── DTOs/            # Data Transfer Objects
│   └── Enums/           # Enumerations
├── Data/                # DbContext và configurations
├── Services/            # Business logic layer
├── Repositories/        # Data access layer
├── Extensions/          # Extension methods và configurations
├── Middleware/          # Custom middleware
└── Migrations/          # EF Core migrations
```

## Models và Data Transfer Objects

### Product Entity
```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }           // Required, Max 200 chars
    public string? Description { get; set; }   // Optional
    public decimal StockPrice { get; set; }    // Required, decimal(10,2)
    public decimal Price { get; set; }         // Required, decimal(10,2)
    public int StockQuantity { get; set; }     // Required, >= 0
    public Guid? CategoryId { get; set; }      // Optional foreign key
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation Properties
    public virtual Category? Category { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; }
    public virtual ICollection<PromotionProduct> PromotionProducts { get; set; }
}
```

### Category Entity
```csharp
public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }           // Required, Max 100 chars
    public string? Description { get; set; }   // Optional
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation Properties
    public virtual ICollection<Product> Products { get; set; }
}
```

### API Response Format
Tất cả API endpoints trả về dữ liệu theo format chuẩn:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}
```

### Phân Trang (Pagination)
```csharp
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
```

## API Endpoints

### Products API (`/api/products`)

#### 1. Lấy Danh Sách Sản Phẩm
**GET** `/api/products`

**Query Parameters:**
- `page` (int, default: 1): Số trang
- `pageSize` (int, default: 10): Số items per page
- `searchTerm` (string, optional): Tìm kiếm theo tên
- `categoryId` (Guid, optional): Lọc theo danh mục
- `minPrice` (decimal, optional): Giá tối thiểu
- `maxPrice` (decimal, optional): Giá tối đa
- `sortBy` (string, default: "Name"): Sắp xếp theo field
- `sortOrder` (string, default: "asc"): Thứ tự sắp xếp (asc/desc)

**Response:** `ApiResponse<PagedResult<ProductDto>>`

**Ví dụ Request:**
```http
GET /api/products?page=1&pageSize=5&searchTerm=giày&sortBy=Price&sortOrder=desc
```

**Ví dụ Response:**
```json
{
  "success": true,
  "message": "Thành công",
  "data": {
    "items": [
      {
        "id": "123e4567-e89b-12d3-a456-426614174000",
        "name": "Giày thể thao Nike",
        "description": "Giày thể thao cao cấp",
        "stockPrice": 800000,
        "price": 1200000,
        "stockQuantity": 50,
        "categoryId": "456e7890-e89b-12d3-a456-426614174001",
        "category": {
          "id": "456e7890-e89b-12d3-a456-426614174001",
          "name": "Giày thể thao",
          "description": "Danh mục giày thể thao"
        },
        "createdAt": "2024-01-15T10:30:00Z",
        "updatedAt": "2024-01-15T10:30:00Z"
      }
    ],
    "totalCount": 25,
    "page": 1,
    "pageSize": 5,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "errors": null
}
```

#### 2. Lấy Sản Phẩm Theo ID
**GET** `/api/products/{id}`

**Path Parameters:**
- `id` (Guid): ID của sản phẩm

**Response:** `ApiResponse<ProductDto>`

**Ví dụ Request:**
```http
GET /api/products/123e4567-e89b-12d3-a456-426614174000
```

**Status Codes:**
- `200 OK`: Thành công
- `404 Not Found`: Không tìm thấy sản phẩm
- `500 Internal Server Error`: Lỗi server

#### 3. Tạo Sản Phẩm Mới
**POST** `/api/products`

**Request Body:** `ProductCreateDto`
```json
{
  "name": "Giày thể thao Adidas",
  "description": "Giày thể thao chất lượng cao",
  "stockPrice": 700000,
  "price": 1100000,
  "stockQuantity": 30,
  "categoryId": "456e7890-e89b-12d3-a456-426614174001"
}
```

**Validation Rules:**
- `name`: Bắt buộc, tối đa 200 ký tự
- `stockPrice`: Bắt buộc, >= 0
- `price`: Bắt buộc, >= 0
- `stockQuantity`: Bắt buộc, >= 0
- `categoryId`: Tùy chọn, phải tồn tại trong database

**Response:** `ApiResponse<ProductDto>`

**Status Codes:**
- `201 Created`: Tạo thành công
- `400 Bad Request`: Dữ liệu không hợp lệ
- `500 Internal Server Error`: Lỗi server

#### 4. Cập Nhật Sản Phẩm
**PUT** `/api/products/{id}`

**Path Parameters:**
- `id` (Guid): ID của sản phẩm

**Request Body:** `ProductUpdateDto`
```json
{
  "name": "Giày thể thao Adidas Updated",
  "description": "Mô tả đã cập nhật",
  "stockPrice": 750000,
  "price": 1150000,
  "stockQuantity": 25,
  "categoryId": "456e7890-e89b-12d3-a456-426614174001"
}
```

**Response:** `ApiResponse<ProductDto>`

**Status Codes:**
- `200 OK`: Cập nhật thành công
- `400 Bad Request`: Dữ liệu không hợp lệ
- `404 Not Found`: Không tìm thấy sản phẩm
- `500 Internal Server Error`: Lỗi server

#### 5. Xóa Sản Phẩm
**DELETE** `/api/products/{id}`

**Path Parameters:**
- `id` (Guid): ID của sản phẩm

**Response:** `ApiResponse<bool>`

**Status Codes:**
- `200 OK`: Xóa thành công
- `404 Not Found`: Không tìm thấy sản phẩm
- `500 Internal Server Error`: Lỗi server

#### 6. Lấy Sản Phẩm Theo Danh Mục
**GET** `/api/products/category/{categoryId}`

**Path Parameters:**
- `categoryId` (Guid): ID của danh mục

**Response:** `ApiResponse<List<ProductDto>>`

**Ví dụ Request:**
```http
GET /api/products/category/456e7890-e89b-12d3-a456-426614174001
```

### Categories API (`/api/categories`)

#### 1. Lấy Danh Sách Danh Mục
**GET** `/api/categories`

**Query Parameters:**
- `page` (int, default: 1): Số trang
- `pageSize` (int, default: 10): Số items per page
- `searchTerm` (string, optional): Tìm kiếm theo tên
- `sortBy` (string, default: "Name"): Sắp xếp theo field
- `sortOrder` (string, default: "asc"): Thứ tự sắp xếp

**Response:** `ApiResponse<PagedResult<CategoryDto>>`

#### 2. Lấy Danh Mục Theo ID
**GET** `/api/categories/{id}`

**Response:** `ApiResponse<CategoryDto>`

#### 3. Tạo Danh Mục Mới
**POST** `/api/categories`

**Request Body:** `CategoryCreateDto`
```json
{
  "name": "Giày cao gót",
  "description": "Danh mục giày cao gót thời trang"
}
```

**Response:** `ApiResponse<CategoryDto>`

#### 4. Cập Nhật Danh Mục
**PUT** `/api/categories/{id}`

**Request Body:** `CategoryUpdateDto`

**Response:** `ApiResponse<CategoryDto>`

#### 5. Xóa Danh Mục
**DELETE** `/api/categories/{id}`

**Response:** `ApiResponse<bool>`

#### 6. Lấy Danh Sách Danh Mục Đơn Giản
**GET** `/api/categories/simple`

**Response:** `ApiResponse<List<CategoryDto>>`

Endpoint này trả về tất cả danh mục không phân trang, thường dùng cho dropdown lists.

## Error Handling

API sử dụng Global Exception Middleware để xử lý lỗi thống nhất:

### Các Loại Lỗi Phổ Biến

1. **Validation Errors (400 Bad Request)**
```json
{
  "success": false,
  "message": "Dữ liệu không hợp lệ",
  "data": null,
  "errors": [
    "Tên sản phẩm là bắt buộc",
    "Giá bán phải lớn hơn hoặc bằng 0"
  ]
}
```

2. **Not Found Errors (404 Not Found)**
```json
{
  "success": false,
  "message": "Không tìm thấy sản phẩm",
  "data": null,
  "errors": null
}
```

3. **Server Errors (500 Internal Server Error)**
```json
{
  "success": false,
  "message": "Lỗi server nội bộ",
  "data": null,
  "errors": null
}
```

## Hướng Dẫn Sử Dụng

### 1. Khởi Chạy API

```bash
# Clone repository
git clone <repository-url>
cd ProductAPI

# Restore packages
dotnet restore

# Update database
dotnet ef database update

# Run API
dotnet run
```

### 2. Truy Cập Swagger UI
Mở trình duyệt và truy cập: `http://localhost:5046`

### 3. Cấu Hình Database
Cập nhật connection string trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TheShoe;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### 4. Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ASPNETCORE_URLS`: Custom URLs (default: http://localhost:5046)

## Ví Dụ Thực Tế

### Tạo Danh Mục và Sản Phẩm

1. **Tạo danh mục mới:**
```http
POST /api/categories
Content-Type: application/json

{
  "name": "Giày thể thao",
  "description": "Danh mục các loại giày thể thao"
}
```

2. **Tạo sản phẩm trong danh mục:**
```http
POST /api/products
Content-Type: application/json

{
  "name": "Nike Air Max 270",
  "description": "Giày thể thao Nike Air Max 270 chính hãng",
  "stockPrice": 1500000,
  "price": 2200000,
  "stockQuantity": 20,
  "categoryId": "CATEGORY_ID_FROM_STEP_1"
}
```

### Tìm Kiếm và Lọc Sản Phẩm

```http
GET /api/products?searchTerm=Nike&minPrice=1000000&maxPrice=3000000&sortBy=Price&sortOrder=asc&page=1&pageSize=10
```

### Cập Nhật Thông Tin Sản Phẩm

```http
PUT /api/products/{productId}
Content-Type: application/json

{
  "name": "Nike Air Max 270 - Updated",
  "description": "Mô tả đã được cập nhật",
  "stockPrice": 1600000,
  "price": 2300000,
  "stockQuantity": 15,
  "categoryId": "CATEGORY_ID"
}
```

## Testing

### Unit Tests
Dự án bao gồm comprehensive unit tests:
- Controller tests
- Service tests  
- Repository tests
- Database schema tests

Chạy tests:
```bash
dotnet test
```

### Postman Collection
Import file `ProductAPI.postman_collection.json` vào Postman để có sẵn các request mẫu.

## Bảo Mật

### Hiện Tại
- CORS được cấu hình cho phép tất cả origins (Development)
- HTTPS redirection được bật
- Input validation thông qua Data Annotations

### Khuyến Nghị Cho Production
- Implement Authentication/Authorization (JWT)
- Restrict CORS policies
- Add rate limiting
- Implement API versioning
- Add request/response logging
- Use HTTPS certificates

## Monitoring và Logging

- Structured logging với Microsoft.Extensions.Logging
- Exception logging trong Global Exception Middleware
- Entity Framework query logging (configurable)

## Phiên Bản và Changelog

**Version 1.0.0**
- Initial release với CRUD operations cho Products và Categories
- Swagger documentation
- Unit tests
- Global exception handling
- AutoMapper integration
- Pagination và filtering

## Integration Examples

### C# Client Example
```csharp
using System.Text.Json;
using System.Text;

public class ProductApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5046";

    public ProductApiClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<ApiResponse<PagedResult<ProductDto>>> GetProductsAsync(ProductQueryDto query)
    {
        var queryString = $"?page={query.Page}&pageSize={query.PageSize}";
        if (!string.IsNullOrEmpty(query.SearchTerm))
            queryString += $"&searchTerm={Uri.EscapeDataString(query.SearchTerm)}";

        var response = await _httpClient.GetAsync($"{_baseUrl}/api/products{queryString}");
        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ApiResponse<PagedResult<ProductDto>>>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public async Task<ApiResponse<ProductDto>> CreateProductAsync(ProductCreateDto product)
    {
        var json = JsonSerializer.Serialize(product, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/products", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ApiResponse<ProductDto>>(responseJson, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
```

### JavaScript/TypeScript Example
```typescript
interface ApiResponse<T> {
    success: boolean;
    message: string;
    data?: T;
    errors?: string[];
}

interface ProductDto {
    id: string;
    name: string;
    description?: string;
    stockPrice: number;
    price: number;
    stockQuantity: number;
    categoryId?: string;
    category?: CategoryDto;
    createdAt: string;
    updatedAt: string;
}

interface ProductCreateDto {
    name: string;
    description?: string;
    stockPrice: number;
    price: number;
    stockQuantity: number;
    categoryId?: string;
}

class ProductApiClient {
    private baseUrl = 'http://localhost:5046';

    async getProducts(query: {
        page?: number;
        pageSize?: number;
        searchTerm?: string;
        categoryId?: string;
        minPrice?: number;
        maxPrice?: number;
        sortBy?: string;
        sortOrder?: string;
    } = {}): Promise<ApiResponse<PagedResult<ProductDto>>> {
        const params = new URLSearchParams();
        Object.entries(query).forEach(([key, value]) => {
            if (value !== undefined) {
                params.append(key, value.toString());
            }
        });

        const response = await fetch(`${this.baseUrl}/api/products?${params}`);
        return await response.json();
    }

    async createProduct(product: ProductCreateDto): Promise<ApiResponse<ProductDto>> {
        const response = await fetch(`${this.baseUrl}/api/products`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(product),
        });
        return await response.json();
    }

    async updateProduct(id: string, product: ProductCreateDto): Promise<ApiResponse<ProductDto>> {
        const response = await fetch(`${this.baseUrl}/api/products/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(product),
        });
        return await response.json();
    }

    async deleteProduct(id: string): Promise<ApiResponse<boolean>> {
        const response = await fetch(`${this.baseUrl}/api/products/${id}`, {
            method: 'DELETE',
        });
        return await response.json();
    }
}
```

### Python Example
```python
import requests
import json
from typing import Optional, Dict, Any

class ProductApiClient:
    def __init__(self, base_url: str = "http://localhost:5046"):
        self.base_url = base_url
        self.session = requests.Session()
        self.session.headers.update({"Content-Type": "application/json"})

    def get_products(self, page: int = 1, page_size: int = 10,
                    search_term: Optional[str] = None,
                    category_id: Optional[str] = None,
                    min_price: Optional[float] = None,
                    max_price: Optional[float] = None,
                    sort_by: str = "Name",
                    sort_order: str = "asc") -> Dict[str, Any]:

        params = {
            "page": page,
            "pageSize": page_size,
            "sortBy": sort_by,
            "sortOrder": sort_order
        }

        if search_term:
            params["searchTerm"] = search_term
        if category_id:
            params["categoryId"] = category_id
        if min_price is not None:
            params["minPrice"] = min_price
        if max_price is not None:
            params["maxPrice"] = max_price

        response = self.session.get(f"{self.base_url}/api/products", params=params)
        return response.json()

    def create_product(self, product_data: Dict[str, Any]) -> Dict[str, Any]:
        response = self.session.post(
            f"{self.base_url}/api/products",
            data=json.dumps(product_data)
        )
        return response.json()

    def get_product(self, product_id: str) -> Dict[str, Any]:
        response = self.session.get(f"{self.base_url}/api/products/{product_id}")
        return response.json()

    def update_product(self, product_id: str, product_data: Dict[str, Any]) -> Dict[str, Any]:
        response = self.session.put(
            f"{self.base_url}/api/products/{product_id}",
            data=json.dumps(product_data)
        )
        return response.json()

    def delete_product(self, product_id: str) -> Dict[str, Any]:
        response = self.session.delete(f"{self.base_url}/api/products/{product_id}")
        return response.json()

# Usage example
if __name__ == "__main__":
    client = ProductApiClient()

    # Get products
    products = client.get_products(page=1, page_size=5, search_term="Nike")
    print(f"Found {products['data']['totalCount']} products")

    # Create new product
    new_product = {
        "name": "Nike Air Force 1",
        "description": "Classic basketball shoe",
        "stockPrice": 1200000,
        "price": 1800000,
        "stockQuantity": 25
    }

    result = client.create_product(new_product)
    if result["success"]:
        print(f"Created product with ID: {result['data']['id']}")
    else:
        print(f"Error: {result['message']}")
```

## Troubleshooting

### Các Vấn Đề Thường Gặp

#### 1. Database Connection Issues
**Lỗi:** `Cannot open database "TheShoe" requested by the login`

**Giải pháp:**
```bash
# Kiểm tra SQL Server service
services.msc -> SQL Server (SQLEXPRESS02)

# Tạo database nếu chưa có
dotnet ef database update

# Hoặc chạy script SQL
sqlcmd -S MYLAPTOP\SQLEXPRESS02 -i theshoe_mssql_create_db.sql
```

#### 2. Port Already in Use
**Lỗi:** `Unable to bind to http://localhost:5046`

**Giải pháp:**
```bash
# Kiểm tra port đang sử dụng
netstat -ano | findstr :5046

# Thay đổi port trong launchSettings.json hoặc
dotnet run --urls "http://localhost:5047"
```

#### 3. CORS Issues
**Lỗi:** `Access to fetch at 'http://localhost:5046' from origin 'http://localhost:3000' has been blocked by CORS policy`

**Giải pháp:** API đã cấu hình CORS cho phép tất cả origins. Nếu vẫn gặp lỗi, kiểm tra:
- Browser cache
- Proxy settings
- Network firewall

#### 4. Validation Errors
**Lỗi:** `400 Bad Request` với validation messages

**Giải pháp:** Kiểm tra request body theo đúng format:
```json
{
  "name": "Required, max 200 chars",
  "stockPrice": "Required, >= 0",
  "price": "Required, >= 0",
  "stockQuantity": "Required, >= 0"
}
```

#### 5. Entity Framework Migration Issues
**Lỗi:** `No migrations configuration type was found`

**Giải pháp:**
```bash
# Add migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Reset database (nếu cần)
dotnet ef database drop
dotnet ef database update
```

### Debug Mode

Để bật debug logging, cập nhật `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Performance Monitoring

Để monitor performance, thêm vào `Program.cs`:
```csharp
// Add response time logging
app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    await next();
    stopwatch.Stop();

    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request {Method} {Path} took {ElapsedMilliseconds}ms",
        context.Request.Method,
        context.Request.Path,
        stopwatch.ElapsedMilliseconds);
});
```

## Deployment

### Development Environment
```bash
# Clone và setup
git clone <repository-url>
cd ProductAPI
dotnet restore
dotnet ef database update
dotnet run
```

### Production Deployment

#### 1. IIS Deployment
```xml
<!-- web.config -->
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath="dotnet" arguments=".\ProductAPI.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" />
  </system.webServer>
</configuration>
```

#### 2. Docker Deployment
```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ProductAPI.csproj", "."]
RUN dotnet restore "./ProductAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ProductAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductAPI.dll"]
```

```bash
# Build và run Docker container
docker build -t productapi .
docker run -d -p 8080:80 --name productapi-container productapi
```

#### 3. Azure App Service
```bash
# Publish to Azure
dotnet publish -c Release
# Upload publish folder to Azure App Service
# Configure connection string in Azure portal
```

### Environment Configuration

#### Production appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=PROD_SERVER;Database=TheShoe;User Id=api_user;Password=secure_password;TrustServerCertificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "yourdomain.com"
}
```

---

*Tài liệu này được tạo tự động từ source code và sẽ được cập nhật theo phiên bản API.*
