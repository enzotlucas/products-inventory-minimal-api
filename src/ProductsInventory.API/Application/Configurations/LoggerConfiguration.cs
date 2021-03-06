namespace ProductsInventory.API.Application.Configurations
{
    public class LoggerConfiguration : IDefinition
    {
        public int ConfigurationOrder => 1;

        public void DefineActions(WebApplication app)
        {
            app.UseLoggingMiddleware();

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecksUI(options => { options.UIPath = "/health-ui"; });
        }

        public void DefineServices(WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                            .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

            builder.Services.AddHealthChecksUI().AddInMemoryStorage();

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                        sinkOptions: new MSSqlServerSinkOptions()
                        {
                            AutoCreateSqlTable = true,
                            TableName = "LogAPI"
                        });
            });
        }
    }
}
