# Products Inventoy Minimal API (.net 6)
A showcase of a simple CRUD Minimal API project

## The project
The project is made as a study showcase, where i'm using diferent libraries and ways of solving problems.

In the API project, i'm using .NET 6, AutoMapper, FluentValidations, Identity, Swashbuckle, EntityFrameworkCore, Dapper and Serilog.
In the tests project, i'm using xUnit, NSubstitute and FluentAssertions.

## Preparing the environment:
First, we need to create the database, using this commands:

```bash
# PowerShell:
Add-Migration InitialDatabase -Context ProductsContext -o "Infrastructure/Data/Migrations"
Add-Migration InitialIdentity -Context IdentityContext -o "Infrastructure/Identity/Migrations"

Update-Database -Context ProductsContext
Update-Database -Context IdentityContext
```

## Dependency injection

The project have an interface called IDefinition, where every class that is derived from it is mapping endpoints or services for the app.
Those endpoints and services are captured by ApplicationConfigurationsExtensions.cs using the assembly, then mapped for the application in the defined order (because of the middlewares, we need to define the order).
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
                                                    .Select(Activator.CreateInstance).Cast<IDefinition>().OrderBy(c => c.ConfigurationOrder));

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

The interface basically defines two methods and the ConfigurationOrder property:

```cs
namespace ProductsInventory.API.Core.DomainObjects
{
    public interface IDefinition
    {
        int ConfigurationOrder { get; }
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
        public int ConfigurationOrder => 3;

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
The order here is important for the middlewares, like the logging middleware, it haves to be one of the first.

## Diferent endpoints mappings

In this project, i'm using 2 ways of endpoint mapping.

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
The application logs are made using Segilog and saving in the database (table name is LogAPI). The configuration is in **LoggerConfiguration.cs**.<br>
The app have a LoggingMiddleware to log every request there are not from healthchecks.

**LoggerConfiguration.cs**
```cs
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
```

**LoggerMiddleware.cs**
```cs
public class LoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggerMiddleware> _logger;

    public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await HandleLogsAsync(context);

        await _next(context);
    }

    private Task HandleLogsAsync(HttpContext context)
    {
        if (context.Request.Path.Equals("/health"))
            return Task.CompletedTask;

        if (context.User is null || !context.User.Identity.IsAuthenticated)
        {
            _logger.LogInformation($"Request to route {context.Request.Path} at: {DateTime.Now}.");
            return Task.CompletedTask;
        }

        var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        _logger.LogInformation($"User {userId} requested {context.Request.Path} at: {DateTime.Now}.");
        
        return Task.CompletedTask;
    }
}
```

## Healthchecks
```
<under development>
```

## Security
```
<under development>
```
