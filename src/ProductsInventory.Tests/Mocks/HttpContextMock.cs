namespace ProductsInventory.Tests.Mocks
{
    public static class HttpContextMock
    {
        public static HttpContext GenerateAuthenticateduserHttpContext(UserType userType)
        {
            var identity = new ClaimsIdentity("authMethod");

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));

            foreach (var claim in GenerateClaimByUserType(userType).ToList())
                identity.AddClaim(claim);

            var contextUser = new ClaimsPrincipal(identity);

            return new DefaultHttpContext()
            {
                User = contextUser
            };
        }

        private static IEnumerable<Claim> GenerateClaimByUserType(UserType userType)
        {
            return userType switch
            {
                UserType.ADMINISTRATOR => new List<Claim>
                    {
                        new Claim("Products", "Create"),
                        new Claim("Products", "Read"),
                        new Claim("Products", "Update"),
                        new Claim("Products", "Delete")
                    },
                UserType.SELLER => new List<Claim>
                    {
                        new Claim("Products", "Read"),
                        new Claim("Products", "Update")
                    },
                _ => throw new Exception("Only valid UserTypes"),
            };
        }

        public static HttpContext GenerateDefaultHttpContext()
        {
            return new DefaultHttpContext();
        }
    }
}
