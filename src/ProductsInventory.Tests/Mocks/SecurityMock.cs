namespace ProductsInventory.Tests.Mocks
{
    public static class SecurityMock
    {
        public static UserManager<IdentityUser> GenerateValidUserManager()
        {
            var userStoreMock = Substitute.For<IUserStore<IdentityUser>>();

            return Substitute.For<UserManager<IdentityUser>>(userStoreMock, null, null, null, null, null, null, null, null);
        }

        public static CreateAccountRequest GenerateValidCreateAccountRequest(UserType userType)
        {
            return new CreateAccountRequest("user@email.com", "P@ssw0rd", "User Name", userType);
        }

        public static CreateAccountRequest GenerateInvalidCreateAccountRequest(UserType userType, bool invalidEmail = false, bool invalidPassord = false, bool invalidName = false)
        {
            var email = invalidEmail ? string.Empty : "user@email.com";
            var password = invalidPassord ? string.Empty : "P@ssw0rd";
            var name = invalidName ? string.Empty : "User Name";

            return new CreateAccountRequest(email, password, name, userType);
        }

        public static AccessTokenResponse GenerateAccessToken()
        {
            var key = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Audience = "audience@email.com",
                Issuer = "issuer@email.com",
                Expires = DateTime.UtcNow.AddSeconds(300)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AccessTokenResponse("Bearer", tokenHandler.WriteToken(token), tokenDescriptor.Expires.Value, Guid.Empty.ToString());
        }

        public static WebApplicationBuilder GenerateValidAplicationBuilder()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Configuration["Jwt:Key"] = Guid.NewGuid().ToString();
            builder.Configuration["Jwt:Audience"] = "email@email.com";
            builder.Configuration["Jwt:Issuer"] = "email@email.com";
            return builder;
        }
    }
}
