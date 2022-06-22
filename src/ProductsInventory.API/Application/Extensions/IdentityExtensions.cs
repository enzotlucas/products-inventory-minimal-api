﻿namespace ProductsInventory.API.Application.Extensions
{
    public static class IdentityExtensions
    {
        public static async Task<IResult> CreateUserAsync(this UserManager<IdentityUser> userManager, CreateAccountRequest dto)
        {
            if (dto is null)
                return Results.BadRequest("Invalid credentials");

            var newUser = new IdentityUser { UserName = dto.Email, Email = dto.Email };

            var userCreated = await userManager.CreateAsync(newUser, dto.Password);

            if (!userCreated.Succeeded)
                return Results.ValidationProblem(userCreated.Errors.ConvertToProblemDetails());

            var claimsCreated = await CreateUserClaimsAsync(dto, userManager, newUser);

            if (!claimsCreated.Succeeded)
                return Results.ValidationProblem(claimsCreated.Errors.ConvertToProblemDetails());

            return Results.Ok(newUser);
        }

        private static async Task<IdentityResult> CreateUserClaimsAsync(CreateAccountRequest dto, UserManager<IdentityUser> userManager, IdentityUser newUser)
        {
            var userClaims = GenerateClaimsByUserType(dto);

            return await userManager.AddClaimsAsync(newUser, userClaims);
        }

        private static IEnumerable<Claim> GenerateClaimsByUserType(CreateAccountRequest dto)
        {
            var claimList = new List<Claim>
            {
                new Claim("Name",dto.Name),
                new Claim("UserType",dto.UserType.ToString())
            };

            switch (dto.UserType)
            {
                case UserType.ADMINISTRATOR:
                    claimList.Add(new Claim("Products", "Create"));
                    claimList.Add(new Claim("Products", "Read"));
                    claimList.Add(new Claim("Products", "Update"));
                    claimList.Add(new Claim("Products", "Delete"));
                    break;
                case UserType.SELLER:
                    claimList.Add(new Claim("Products", "Read"));
                    claimList.Add(new Claim("Products", "Update"));
                    break;
                default:
                    break;
            }

            return claimList;
        }

        public static async Task<HttpContext> GetResposeValue(this IResult response)
        {
            var context = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider(),
                Response =
                {
                    Body = new MemoryStream(),
                },
            };

            await response.ExecuteAsync(context);

            return context;
        }

        public static async Task<T> GetObjectFromBody<T>(this HttpContext context)
        {
            return await JsonSerializer.DeserializeAsync<T>(context.Response.Body, 
                                                                      new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }

        public static AccessTokenResponse GenerateToken(this IdentityUser user, ConfigurationManager configuration, ClaimsIdentity claims)
        {
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Audience = configuration["Jwt:Audience"],
                Issuer = configuration["Jwt:Issuer"],
                Expires = DateTime.UtcNow.AddSeconds(300)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AccessTokenResponse("Bearer", tokenHandler.WriteToken(token), tokenDescriptor.Expires.Value, user.Id);
        }
    }
}
