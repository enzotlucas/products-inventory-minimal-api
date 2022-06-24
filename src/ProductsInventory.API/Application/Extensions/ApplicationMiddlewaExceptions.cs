using ProductsInventory.API.Application.Configurations.Middlewares;

namespace ProductsInventory.API.Application.Extensions
{
    public static class ApplicationMiddlewaExceptions
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        public static void UseLoggingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggerMiddleware>();
        }
    }
}
