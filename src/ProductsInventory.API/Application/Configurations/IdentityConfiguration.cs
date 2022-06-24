namespace ProductsInventory.API.Application.Configurations
{
    public class IdentityConfiguration : IDefinition
    {
        public int OrderPriority => 2;

        public void DefineActions(WebApplication app)
        {
            app.UseAuthentication();

            app.UseAuthorization();
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
            ).AddEntityFrameworkStores<IdentityContext>()
             .AddDefaultTokenProviders();

            builder.Services.AddAuthorization();

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
        }
    }
}
