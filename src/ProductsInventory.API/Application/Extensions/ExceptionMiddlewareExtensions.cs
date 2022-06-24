using ProductsInventory.API.Application.Configurations.Middlewares;

namespace ProductsInventory.API.Application.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
