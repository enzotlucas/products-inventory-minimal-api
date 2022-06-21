# products-inventory-minimal-api
A CRUD API using Minimal API

## Creating the database (adding Migrations):

```bash
# PowerShell:
Add-Migrations InitialDatabase -Context ProductsContext -o "Infrastructure/Data/Migrations"
Add-Migrations InitialIdentity -Context IdentityContext -o "Infrastructure/Identity/Migrations"

Update-Database -Context ProductsContext
Update-Database -Context IdentityContext
```

## Dependency injections

The project have an interface called IDefinition, where every class that is derived from it is mapping endpoints or services for the app.
Those endpoints and services are captured by ApplicationConfigurationsExtensions.cs using the assembly, then mapped for the application.
The class ApplicationConfigurationsExtensions.cs provides the methods that are used in the Program.cs, as the examples shows:

**ApplicationConfigurationsExtensions.cs**:
```cs
namespace ProductsInventory.API.Application.Configurations
{
    public static class ApplicationConfigurationsExtensions
    {
        public static void AddApplicationConfigurations(this WebApplicationBuilder builder, params Type[] scanMarkers)
        {
            var configurations = new List<IDefinition>();

            foreach (var scanMarker in scanMarkers)
                configurations.AddRange(scanMarker.Assembly.ExportedTypes
                                                    .Where(e => typeof(IDefinition).IsAssignableFrom(e) && e.BaseType is not null)
                                                    .Select(Activator.CreateInstance).Cast<IDefinition>());

            foreach (var configuration in configurations)
                configuration.DefineServices(builder);

            builder.Services.AddSingleton(configurations as IReadOnlyCollection<IDefinition>);
        }

        public static void UseApplicationConfigurations(this WebApplication app)
        {
            var configurations = app.Services.GetRequiredService<IReadOnlyCollection<IDefinition>>();

            foreach (var configuration in configurations)
                configuration.DefineActions(app);
        }
    }
}
```

**Program.cs**:

```cs
var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationConfigurations(typeof(Program));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseApplicationConfigurations();

app.Run();
```

The interface basically defines two methods:

```cs
namespace ProductsInventory.API.Core.DomainObjects
{
    public interface IDefinition
    {
        void DefineActions(WebApplication app);
        void DefineServices(WebApplicationBuilder builder);
    }
}
```

And you can use to map the endpoints like: 
```cs
public void DefineActions(WebApplication app)
{
    app.Map("/error", (HttpContext http) =>
    {
        var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;
        if (error is null)
            return Results.Problem("An error ocurred", statusCode: 500);
        if (error is BusinessException)
            return Results.BadRequest(error.Message);
        return Results.Problem($"An error ocurred, message: {error.InnerException?.Message ?? error.Message}", statusCode: 500);
    });
}
```

Or map with delegates:
```cs
 public void DefineActions(WebApplication app)
 {
     app.MapPost("/account/", CreateAccountAsync)
        .WithTags("Account")
        .ProducesValidationProblem()
        .Produces<AccessTokenViewModel>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
 }

 internal async Task<IResult> CreateAccountAsync(
                ILogger<SecurityEndpointsConfiguration> logger,
                HttpContext context, 
                UserManager<IdentityUser> userManager, 
                UserViewModel dto)
        {
            //Some code here
        }
```

And you can use to map your services, like Swagger:

**SwaggerConfiguration.cs**:
```cs
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

namespace ProductsInventory.API.Application.Configurations
{
    public class SwaggerConfiguration : IDefinition
    {
        public void DefineActions(WebApplication app)
        {
            app.UseSwagger();

            app.UseSwaggerUI();
        }

        public void DefineServices(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();
        }
    }
}
```