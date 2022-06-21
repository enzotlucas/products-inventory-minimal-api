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

            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(SchemaIdStrategy);

                options.DescribeAllParametersInCamelCase();

                options.SchemaFilter<EnumSchemaFilter>();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Use the JWT token in the format: Bearer (your token)",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        private static string SchemaIdStrategy(Type currentClass)
        {
            string returnedValue = currentClass.Name;
            if (returnedValue.EndsWith("ViewModel"))
                returnedValue = returnedValue.Replace("ViewModel", string.Empty);
            return returnedValue;
        }
    }
}