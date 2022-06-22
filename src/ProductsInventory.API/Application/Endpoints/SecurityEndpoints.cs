namespace ProductsInventory.API.Application.Endpoints
{
    public class SecurityEndpoints : IDefinition
    {
        private ConfigurationManager _configuration;

        public void DefineActions(WebApplication app)
        {
            app.MapPost("/api/account/", CreateAccountAsync)
               .WithTags("Account")
               .ProducesValidationProblem()
               .Produces<AccessTokenResponse>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapPost("/api/account/login", LoginAsync)
               .WithTags("Account")
               .ProducesValidationProblem()
               .Produces<AccessTokenResponse>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest);
        }

        [AllowAnonymous]
        internal async Task<IResult> CreateAccountAsync(ILogger<SecurityEndpoints> logger, 
                                                        HttpContext context, 
                                                        UserManager<IdentityUser> userManager, 
                                                        CreateAccountRequest dto)
        {
            var userCreated = await userManager.CreateUserAsync(dto).Result.GetResposeValue();

            if (userCreated is not null && userCreated.Response.StatusCode is not 200)
                return logger.LogAndResponse(userCreated, dto);

            var newUser = await userCreated.GetObjectFromBody<IdentityUser>();

            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, dto.Email),
                new Claim(ClaimTypes.NameIdentifier, newUser.Id),
            });

            claims.AddClaims(await userManager.GetClaimsAsync(newUser));

            var response = newUser.GenerateToken(_configuration, claims);

            return logger.OkWithLog($"Create account success, userId: {newUser.Id}", response);
        }

        [AllowAnonymous]
        internal async Task<IResult> LoginAsync(ILogger<SecurityEndpoints> logger, 
                                                UserManager<IdentityUser> userManager, 
                                                LoginRequest dto)
        {
            if (dto is null)
                return logger.BadRequestWithLog("Invalid credentials", $", username: {dto.Username}");

            var user = userManager.FindByEmailAsync(dto.Username).Result;
            
            if (user is null || !await userManager.CheckPasswordAsync(user, dto.Password))
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

            var response = user.GenerateToken(_configuration, new ClaimsIdentity(claims));

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