using ProductAPI.Models.DTOs;
using System.Net;
using System.Text.Json;

namespace ProductAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi không mong muốn: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new ApiResponse<object>();

            switch (exception)
            {
                case ArgumentNullException:
                    response = ApiResponse<object>.ErrorResult("Tham số không được để trống");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                
                case ArgumentException:
                    response = ApiResponse<object>.ErrorResult("Tham số không hợp lệ");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                
                case UnauthorizedAccessException:
                    response = ApiResponse<object>.ErrorResult("Không có quyền truy cập");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                
                case KeyNotFoundException:
                    response = ApiResponse<object>.ErrorResult("Không tìm thấy tài nguyên");
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                
                case InvalidOperationException:
                    response = ApiResponse<object>.ErrorResult("Thao tác không hợp lệ");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                
                default:
                    response = ApiResponse<object>.ErrorResult("Đã xảy ra lỗi server nội bộ");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
