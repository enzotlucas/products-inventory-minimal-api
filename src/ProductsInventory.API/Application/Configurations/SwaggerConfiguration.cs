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

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = builder.Configuration["Swagger:Title"],
                    Description = builder.Configuration["Swagger:Description"],
                    TermsOfService = new Uri(builder.Configuration["Swagger:TermsOfService"]),
                    Contact = new OpenApiContact
                    {
                        Name = builder.Configuration["Swagger:Contact:Name"],
                        Url = new Uri(builder.Configuration["Swagger:Contact:Url"])
                    },
                    License = new OpenApiLicense
                    {
                        Name = builder.Configuration["Swagger:License:Name"],
                        Url = new Uri(builder.Configuration["Swagger:License:Url"])
                    }
                });

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
            if(returnedValue.EndsWith("Response"))
                returnedValue = returnedValue.Replace("Response", string.Empty);
            return returnedValue;
        }
    }
}