namespace ProductsInventory.API.Application.Configurations
{
    public class IdentityConfiguration : IDefinition
    {
        public void DefineActions(WebApplication app)
        {
            app.UseAuthorization();

            app.UseAuthentication();
        }

        public void DefineServices(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<IdentityContext>(i =>
                         i.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                                       options => options.EnableRetryOnFailure(6)));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            }
            ).AddEntityFrameworkStores<IdentityContext>();

            //builder.Services.AddAuthorization(options =>
            //{
            //    //options.FallbackPolicy = new AuthorizationPolicyBuilder()
            //    //  .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            //    //  //.RequireAuthenticatedUser()
            //    //  .Build();

            //    options.AddPolicy("CanCreateProducts", p =>
            //        p.RequireAuthenticatedUser().RequireClaim("Products", "Create"));

            //    options.AddPolicy("CanReadProducts", p =>
            //        p.RequireAuthenticatedUser().RequireClaim("Products", "Read"));

            //    options.AddPolicy("CanUpdateProducts", p =>
            //        p.RequireAuthenticatedUser().RequireClaim("Products", "Update"));

            //    options.AddPolicy("CanDeleteProducts", p =>
            //        p.RequireAuthenticatedUser().RequireClaim("Products", "Delete"));
            //});

            builder.Services.AddAuthorization();

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"]
                };
            });
        }
    }
}
