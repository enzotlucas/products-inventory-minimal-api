namespace ProductsInventory.API.Application.Configurations
{
    public class LoggerConfiguration : IDefinition
    {
        public void DefineActions(WebApplication app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            }
            );

            app.MapHealthChecksUI();
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
