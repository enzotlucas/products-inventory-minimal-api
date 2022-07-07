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

        public static WebApplicationBuilder GenerateValidAplicationBuilder()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Configuration["Jwt:Key"] = Guid.NewGuid().ToString();
            builder.Configuration["Jwt:Audience"] = "email@email.com";
            builder.Configuration["Jwt:Issuer"] = "email@email.com";
            return builder;
        }

        public static IEnumerable<IdentityUser> GenerateValidIdentityUsers(int quantity)
        {
            if (quantity < 1)
                throw new Exception("Quantity can't be lower than 1");

            var users = new List<IdentityUser>();

            for (int i = 0; i < quantity; i++)
                users.Add(GenerateValidIdentityUser($"user{i}"));

            return users;
        }

        public static IdentityUser GenerateValidIdentityUser(string email)
        {
            return new IdentityUser
            {
                Email = email,
                UserName = email
            };
        }

        public static IEnumerable<UserResponse> GenerateValidUserResponseList(IEnumerable<IdentityUser> users)
        {
            var response = new List<UserResponse>();

            foreach (var user in users)
                response.Add(ConvertToUserResponse(user));

            return response;
        }

        public static UserResponse ConvertToUserResponse(IdentityUser user)
        {
            return new UserResponse(Guid.Parse(user.Id), user.Email);
        }

        public static IList<Claim> GenerateValidClaimsByUserTyoe(UserType userType)
        {
            var response = new List<Claim>
            {
                new Claim("Name", "User Name"),
                new Claim("UserType", userType.ToString())
            };

            switch (userType)
            {
                case UserType.ADMINISTRATOR:
                    response.Add(new Claim("Products", "Create"));
                    response.Add(new Claim("Products", "Read"));
                    response.Add(new Claim("Products", "Update"));
                    response.Add(new Claim("Products", "Delete"));
                    break;
                case UserType.SELLER:
                    response.Add(new Claim("Products", "Read"));
                    response.Add(new Claim("Products", "Update"));
                    break;
                default:
                    break;
            }

            return response;
        }
    }
}
