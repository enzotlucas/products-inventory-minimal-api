using Newtonsoft.Json;
using System.Net;

namespace ProductsInventory.API.Application.Configurations.Middlewares
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

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex.Message, ex.ValidationErrors);

                await HandleExceptionAsync(httpContext, ex).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error ocurred, exception: {ex}", ex);

                await HandleExceptionAsync(httpContext, ex).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, BusinessException exception)
        {
            context.Response.ContentType = "application/json";

            var result = exception.ValidationErrors.Count > 0 ?
                JsonConvert.SerializeObject(new { Message = "Invalid product", Errors = exception.ValidationErrors }) :
                JsonConvert.SerializeObject(new { exception.Message });

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return context.Response.WriteAsync(result);
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonConvert.SerializeObject(
                new { StatusCode = statusCode, ErrorMessage = exception.Message });

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}
