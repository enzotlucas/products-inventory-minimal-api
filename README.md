# products-inventory-minimal-api
A CRUD API using Minimal API

## Preparing the environment:

```bash
# PowerShell:
Install-Package Microsoft.EntityFrameworkCore

Add-Migration InitialDatabase -Context ProductsContext -o "Infrastructure/Data/Migrations"
Add-Migration InitialIdentity -Context IdentityContext -o "Infrastructure/Identity/Migrations"

Update-Database -Context ProductsContext
Update-Database -Context IdentityContext
```

## Dependency injection

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

## Diferent endpoints mappings

In this project, i'm using 3 ways of endpoint mapping.

### **- The "normal" way**:
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

### **- With a internal method delegate**:
```cs
public void DefineActions(WebApplication app)
{
    app.MapPost("/account/", CreateAccountAsync)
       .WithTags("Account")
       .ProducesValidationProblem()
       .Produces<AccessTokenViewModel>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status400BadRequest);
}

[AllowAnonymous]
internal async Task<IResult> CreateAccountAsync(ILogger<SecurityEndpointsConfiguration> logger, 
                                                HttpContext context, 
                                                UserManager<IdentityUser> userManager,
                                                UserViewModel dto)
{
    //Some  code
}
```

### **- With a handler class**:
```cs
public void DefineActions(WebApplication app)
{
    app.MapMethods(CreateProduct.Route, CreateProduct.Methods, CreateProduct.Handle)
       .WithTags("Products")
       .ProducesValidationProblem()
       .Produces<ProductViewModel>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status400BadRequest);
}
```

**CreateProduct.cs**:
```cs
namespace ProductsInventory.API.Application.Commands
{
    public class CreateProduct
    {
        public static string Route => "/products";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [Authorize]
        [ClaimsAuthorize("Products", "Create")]
        public static async Task<IResult> Action(HttpContext context, IProductsRepository repository, ILogger<CreateProduct> logger, ProductViewModel dto)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            logger.LogInformation(string.Format("User {0} requested {1} with payload: {2}", userId, Route, dto.ToString()));

            var product = dto.ToEntity();

            await repository.CreateAsync(product);

            if (!await repository.UnitOfWork.Commit())
                return logger.ProblemWithLog("Error on creating the product", $", userId: {userId}, payload: {dto}");

            return logger.OkWithLog($"Product created by {userId}, product id: {product.Id}", new ProductViewModel(product));
        }
    }
}
```

## Logging
```
<under development>
```

## Healthchecks
```
<under development>
```
