namespace ProductsInventory.Tests.Endpoints.Security
{
    public class CreateAccountTests
    {
        private readonly SecurityEndpoints _sut = new();

        [Trait("CreateAccount", "Security")]
        [Fact(DisplayName = "Create account with a valid user")]
        public async Task CreateAccount_ValidUser_ShouldCreateAndReturnValidAccessToken()
        {
            //Arrange
            var builder = SecurityMock.GenerateValidAplicationBuilder();
            _sut.DefineServices(builder);

            var logger = Substitute.For<ILogger<SecurityEndpoints>>();
            var context = HttpContextMock.GenerateDefaultHttpContext();
            var userManager = SecurityMock.GenerateValidUserManager();
            var userDto = SecurityMock.GenerateValidCreateAccountRequest(UserType.ADMINISTRATOR);

            userManager.CreateAsync(Arg.Any<IdentityUser>(), userDto.Password).Returns(IdentityResult.Success);
            userManager.AddClaimsAsync(Arg.Any<IdentityUser>(), Arg.Any<IEnumerable<Claim>>()).Returns(IdentityResult.Success);

            //Act
            var response = await _sut.CreateAccountAsync(logger, context, userManager, userDto);
            var responseHttpContext = await response.GetResposeHttpContextAsync();
            var responseBody = await responseHttpContext.GetObjectFromBodyAsync<AccessTokenResponse>();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            responseBody.Should().BeAssignableTo<AccessTokenResponse>();
        }

        [Trait("CreateAccount", "Security")]
        [Fact(DisplayName = "Try create account with a invalid e-mail")]
        public async Task CreateAccount_InvalidEmail_ShouldNotCreateAndReturnBadRequest()
        {
            //Arrange
            var builder = SecurityMock.GenerateValidAplicationBuilder();
            _sut.DefineServices(builder);

            var logger = Substitute.For<ILogger<SecurityEndpoints>>();
            var context = HttpContextMock.GenerateDefaultHttpContext();
            var userManager = SecurityMock.GenerateValidUserManager();
            var userDto = SecurityMock.GenerateInvalidCreateAccountRequest(UserType.ADMINISTRATOR, invalidEmail: true);

            userManager.CreateAsync(Arg.Any<IdentityUser>(), userDto.Password).Returns(IdentityResult.Failed(new IdentityError { Description = "Invalid username" }));

            //Act
            var response = await _sut.CreateAccountAsync(logger, context, userManager, userDto);
            var responseHttpContext = await response.GetResposeHttpContextAsync();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Trait("CreateAccount", "Security")]
        [Fact(DisplayName = "Try create account with a invalid password")]
        public async Task CreateAccount_InvalidPassword_ShouldNotCreateAndReturnBadRequest()
        {
            //Arrange
            var builder = SecurityMock.GenerateValidAplicationBuilder();
            _sut.DefineServices(builder);

            var logger = Substitute.For<ILogger<SecurityEndpoints>>();
            var context = HttpContextMock.GenerateDefaultHttpContext();
            var userManager = SecurityMock.GenerateValidUserManager();
            var userDto = SecurityMock.GenerateInvalidCreateAccountRequest(UserType.ADMINISTRATOR, invalidPassord: true);

            userManager.CreateAsync(Arg.Any<IdentityUser>(), userDto.Password).Returns(IdentityResult.Failed(new IdentityError { Description = "Invalid password" }));

            //Act
            var response = await _sut.CreateAccountAsync(logger, context, userManager, userDto);
            var responseHttpContext = await response.GetResposeHttpContextAsync();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Trait("CreateAccount", "Security")]
        [Fact(DisplayName = "Try create account with a invalid user")]
        public async Task CreateAccount_InvalidUser_ShouldNotCreateAndReturnBadRequest()
        {
            //Arrange
            var builder = SecurityMock.GenerateValidAplicationBuilder();
            _sut.DefineServices(builder);

            var logger = Substitute.For<ILogger<SecurityEndpoints>>();
            var context = HttpContextMock.GenerateDefaultHttpContext();
            var userManager = SecurityMock.GenerateValidUserManager();
            CreateAccountRequest? userDto = null;

            //Act
            var response = await _sut.CreateAccountAsync(logger, context, userManager, userDto);
            var responseHttpContext = await response.GetResposeHttpContextAsync();
            var responseBody = await responseHttpContext.GetObjectFromBodyAsync<string>();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            responseBody.Should().Be("Invalid credentials");
        }
    }
}
