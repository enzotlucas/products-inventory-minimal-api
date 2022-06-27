namespace ProductsInventory.API.Application.Configurations
{
    public class AppConfiguration : IDefinition
    {
        public int ConfigurationOrder => 0;

        [AllowAnonymous]
        public void DefineActions(WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsDefaultPolicy");

            app.UseExceptionMiddleware();
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

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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
