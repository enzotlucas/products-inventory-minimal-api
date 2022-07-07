namespace ProductsInventory.Tests.Endpoints.Security
{
    public class LoginTests
    {
        private readonly SecurityEndpoints _sut = new();

        [Trait("Login", "Security")]
        [Fact(DisplayName = "Login with a valid user")]
        public async Task Login_ValidUser_ShouldReturnValidAccessToken()
        {
            //Arrange
            var builder = SecurityMock.GenerateValidAplicationBuilder();
            _sut.DefineServices(builder);

            var logger = Substitute.For<ILogger<SecurityEndpoints>>();
            var userManager = SecurityMock.GenerateValidUserManager();
            var dto = new LoginRequest("username@email.com", "P@ssw0rd");

            var user = SecurityMock.GenerateValidIdentityUser(dto.Username);
            var userClaims = SecurityMock.GenerateValidClaimsByUserTyoe(UserType.ADMINISTRATOR);
            var userRoles = new List<string>();

            userManager.FindByEmailAsync(dto.Username).Returns(user);
            userManager.CheckPasswordAsync(user, dto.Password).Returns(true);
            userManager.GetClaimsAsync(user).Returns(userClaims);
            userManager.GetRolesAsync(user).Returns(userRoles);

            //Act
            var response = await _sut.LoginAsync(logger, userManager, dto);
            var responseHttpContext = await response.GetResposeHttpContextAsync();
            var responseBody = await responseHttpContext.GetObjectFromBodyAsync<AccessTokenResponse>();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            responseBody.Should().BeAssignableTo<AccessTokenResponse>();
        }

        [Trait("Login", "Security")]
        [Fact(DisplayName = "Try to login with invalid login request")]
        public async Task Login_InvalidRequest_ShouldReturnBadRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<SecurityEndpoints>>();
            var userManager = SecurityMock.GenerateValidUserManager();
            LoginRequest dto = null;

            //Act
            var response = await _sut.LoginAsync(logger, userManager, dto);
            var responseHttpContext = await response.GetResposeHttpContextAsync();
            var responseBody = await responseHttpContext.GetObjectFromBodyAsync<string>();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            responseBody.Should().BeAssignableTo<string>();
            responseBody.Should().Be("Invalid credentials");
        }

        [Trait("Login", "Security")]
        [Fact(DisplayName = "Try to login with invalid user")]
        public async Task Login_InvalidUser_ShouldReturnBadRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<SecurityEndpoints>>();
            var userManager = SecurityMock.GenerateValidUserManager();
            var dto = new LoginRequest("username@email.com", "P@ssw0rd");

            IdentityUser user = null;
            userManager.FindByEmailAsync(dto.Username).Returns(user);

            //Act
            var response = await _sut.LoginAsync(logger, userManager, dto);
            var responseHttpContext = await response.GetResposeHttpContextAsync();
            var responseBody = await responseHttpContext.GetObjectFromBodyAsync<string>();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            responseBody.Should().BeAssignableTo<string>();
            responseBody.Should().Be("Invalid credentials");
        }

        [Trait("Login", "Security")]
        [Fact(DisplayName = "Try to login with invalid password")]
        public async Task Login_InvalidPassword_ShouldReturnBadRequest()
        {
            //Arrange
            var logger = Substitute.For<ILogger<SecurityEndpoints>>();
            var userManager = SecurityMock.GenerateValidUserManager();
            var dto = new LoginRequest("username@email.com", "P@ssw0rd");

            var user = SecurityMock.GenerateValidIdentityUser(dto.Username);
            userManager.FindByEmailAsync(dto.Username).Returns(user);
            userManager.CheckPasswordAsync(user, dto.Password).Returns(false);

            //Act
            var response = await _sut.LoginAsync(logger, userManager, dto);
            var responseHttpContext = await response.GetResposeHttpContextAsync();
            var responseBody = await responseHttpContext.GetObjectFromBodyAsync<string>();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            responseBody.Should().BeAssignableTo<string>();
            responseBody.Should().Be("Invalid credentials");
        }
    }
}
