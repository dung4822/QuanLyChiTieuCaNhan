using QuanLyChiTieuCaNhan.CustomExceptions;
using QuanLyChiTieuCaNhan.CustomExceptions.AuthorizationExceptions;
using QuanLyChiTieuCaNhan.CustomExceptions.ResourceExceptions;
using QuanLyChiTieuCaNhan.CustomExceptions.ValidationExceptions;

namespace QuanLyChiTieuCaNhan.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Kiểm tra base class CustomException để lấy mã HTTP
            var statusCode = exception switch
            {
                CustomException customException => customException.StatusCode, // Lấy StatusCode từ CustomException
                _ => 500 // Lỗi không xác định
            };

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = statusCode == 500 ? "An unexpected error occurred." : exception.Message,
                TraceId = context.TraceIdentifier
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Unhandled system exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
            }
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(errorResponse);
        }

    }

}
