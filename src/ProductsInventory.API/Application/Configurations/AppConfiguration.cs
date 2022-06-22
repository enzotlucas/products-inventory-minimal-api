namespace ProductsInventory.API.Application.Configurations
{
    public class AppConfiguration : IDefinition
    {
        [AllowAnonymous]
        public void DefineActions(WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseExceptionHandler("/error");

<<<<<<< HEAD
            app.Map("/error", (HttpContext http, ILogger<AppConfiguration> logger) =>
=======
            app.UseCors("CorsDefaultPolicy");

            app.Map("/error", (HttpContext http) =>
>>>>>>> d29d49d2024e1e07ecbd29ded6e3c5205b6fa218
            {
                var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

                if (error is null)
                {
                    logger.LogCritical("An error ocurred");
                    return Results.Problem("An error ocurred", statusCode: 500);
                }

                if (error is BusinessException)
                {
                    logger.LogWarning(error.Message);
                    return Results.BadRequest(error.Message);
                }

                logger.LogError($"An error ocurred, message: {error.InnerException?.Message ?? error.Message}");
                return Results.Problem($"An error ocurred, message: {error.InnerException?.Message ?? error.Message}", statusCode: 500);
            });
        }

        public void DefineServices(WebApplicationBuilder builder)
        {
            builder.Configuration
                   .SetBasePath(builder.Environment.ContentRootPath)
                   .AddJsonFile("appsettings.json", true, true)
                   .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
                   .AddEnvironmentVariables();

            builder.Services.AddDbContext<ProductsContext>(i =>
              i.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                            options => options.EnableRetryOnFailure(6)));

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("CorsDefaultPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }
    }
}
