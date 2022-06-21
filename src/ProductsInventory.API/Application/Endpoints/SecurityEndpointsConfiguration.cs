using ProductsInventory.API.Core.ValueObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductsInventory.API.Application.Endpoints
{
    public class SecurityEndpointsConfiguration : IDefinition
    {
        private ConfigurationManager _configuration;

        public void DefineActions(WebApplication app)
        {
            app.MapPost("/account/", CreateAccountAsync)
               .WithTags("Account")
               .ProducesValidationProblem()
               .Produces<AccessTokenViewModel>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapPost("/account/login", LoginAsync)
               .WithTags("Account")
               .ProducesValidationProblem()
               .Produces<AccessTokenViewModel>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest);
        }

        [AllowAnonymous]
        internal async Task<IResult> CreateAccountAsync(ILogger<SecurityEndpointsConfiguration> logger, HttpContext context, UserManager<IdentityUser> userManager, UserViewModel dto)
        {
            if (dto is null)
                return logger.BadRequestWithLog("Invalid credentials", $", username: {dto.Email}");

            var newUser = new IdentityUser { UserName = dto.Email, Email = dto.Email };

            var userCreated = await userManager.CreateAsync(newUser, dto.Password);

            if (!userCreated.Succeeded)
                return logger.ValidationProblemsWithLog($"Validations error ocurred, userName: {dto.Email}, errors: {userCreated.Errors.ConvertToProblemDetails()}", userCreated.Errors.ConvertToProblemDetails());

            var claimsCreated = await CreateUserClaimsAsync(dto, userManager, newUser);

            if (!claimsCreated.Succeeded)
            {
                logger.LogWarning($"Bad request on creating account", claimsCreated.Errors.First());

                return Results.BadRequest(claimsCreated.Errors.First());
            }

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, dto.Email),
                new Claim(ClaimTypes.NameIdentifier, newUser.Id),
            });

            subject.AddClaims(await userManager.GetClaimsAsync(newUser));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"],
                Expires = DateTime.UtcNow.AddSeconds(300)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var response = new AccessTokenViewModel("Bearer", tokenHandler.WriteToken(token), tokenDescriptor.Expires.Value, newUser.Id);

            return logger.OkWithLog($"Create account success, userId: {newUser.Id}", response);
        }
        private static async Task<IdentityResult> CreateUserClaimsAsync(UserViewModel dto, UserManager<IdentityUser> userManager, IdentityUser newUser)
        {
            var userClaims = GenerateClaimsByUserType(dto);

            return await userManager.AddClaimsAsync(newUser, userClaims);
        }

        private static IEnumerable<Claim> GenerateClaimsByUserType(UserViewModel dto)
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

        [AllowAnonymous]
        internal async Task<IResult> LoginAsync(ILogger<SecurityEndpointsConfiguration> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signinManager, LoginViewModel dto)
        {
            if (dto is null)
                return logger.BadRequestWithLog("Invalid credentials", $", username: {dto.Username}");

            //if (user is null || !await userManager.CheckPasswordAsync(user, dto.Password))
            //    return logger.BadRequestWithLog("Invalid credentials", $", e-mail: {dto.Username}");

            var signIn = await signinManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);

            var user = await userManager.FindByEmailAsync(dto.Username);

            if (!signIn.Succeeded)
                return logger.BadRequestWithLog("Invalid credentials", $", e-mail: {dto.Username}");

            var claims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
                claims.Add(new Claim("role", userRole));

            var identityClaims = new ClaimsIdentity();

            identityClaims.AddClaims(claims);

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identityClaims,
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"],
                Expires = DateTime.UtcNow.AddSeconds(300)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var response = new AccessTokenViewModel("Bearer", tokenHandler.WriteToken(token), tokenDescriptor.Expires.Value, user.Id);

            return logger.OkWithLog($"Login success, userId: {user.Id}", response);
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        public void DefineServices(WebApplicationBuilder builder)
        {
            _configuration = builder.Configuration;
        }
    }
}