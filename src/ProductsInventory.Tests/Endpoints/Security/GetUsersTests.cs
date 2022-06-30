namespace ProductsInventory.Tests.Endpoints.Security
{
    public class GetUsersTests
    {
        private readonly SecurityEndpoints _sut = new();

        [Trait("GetUsers", "Security")]
        [Fact(DisplayName = "Get all users")]
        public async Task GetUsers_UsersExists_ShouldReturnUsersList()
        {
            //Arrange
            var logger = Substitute.For<ILogger<SecurityEndpoints>>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var userManager = SecurityMock.GenerateValidUserManager();
            var mapper = Substitute.For<IMapper>();

            var users = SecurityMock.GenerateValidIdentityUsers(5).ToList();
            var queryableUsers = new TestAsyncEnumerable<IdentityUser>(users).AsQueryable();
            var userReponses = SecurityMock.GenerateValidUserResponseList(users);

            userManager.Users.Returns(queryableUsers);
            mapper.Map<IEnumerable<UserResponse>>(Arg.Any<IEnumerable<IdentityUser>>()).Returns(userReponses);

            //Act
            var response = await _sut.GetUsersAsync(logger, context, userManager, mapper);
            var responseHttpContext = await response.GetResposeHttpContextAsync();
            var responseBody = await responseHttpContext.GetObjectFromBodyAsync<IEnumerable<UserResponse>>();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            responseBody.Should().NotBeNullOrEmpty();
            responseBody.Should().BeAssignableTo<IEnumerable<UserResponse>>();
            responseBody.Should().BeEquivalentTo(userReponses);
        }
    }
}
