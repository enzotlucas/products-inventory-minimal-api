namespace ProductsInventory.Tests.Endpoints.Products
{
    public class GetAllProductsTests
    {
        [Trait("GetAllProducts", "Products")]
        [Fact(DisplayName = "Get all products with results")]
        public async Task GetAllProducts_ProductsExistsInDatabase_ShouldReturnProductList()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<GetAllProducts>>();
            var mapper = Substitute.For<IMapper>();
            int page = 1;
            int rows = 8;

            var productsList = ProductsMock.GenerateProductList(5);
            repository.GetAllAsync(page, rows).Returns(productsList);

            var expectedResponse = ProductsMock.GenerateProductResponseList(5);
            mapper.Map<IEnumerable<ProductResponse>>(productsList).Returns(expectedResponse);

            //Act
            var response = await GetAllProducts.Action(repository, context, logger, mapper, page, rows);
            var responseHttpContext = await response.GetResposeValueAsync();
            var responseBody = await responseHttpContext.GetObjectFromBodyAsync<IEnumerable<ProductResponse>>();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            responseBody.Should().BeEquivalentTo(expectedResponse);
            responseBody.Should().NotBeNullOrEmpty();
        }

        [Trait("GetAllProducts", "Products")]
        [Fact(DisplayName = "Get a empty list of products")]
        public async Task GetAllProducts_ProductsDontExistsInDatabase_ShouldReturnEmptyProductList()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<GetAllProducts>>();
            var mapper = Substitute.For<IMapper>();
            int page = 1;
            int rows = 8;

            var productsList = new List<Product>();
            repository.GetAllAsync(page, rows).Returns(productsList);

            var expectedResponse = new List<ProductResponse>();
            mapper.Map<IEnumerable<ProductResponse>>(productsList).Returns(expectedResponse);

            //Act
            var response = await GetAllProducts.Action(repository, context, logger, mapper, page, rows);
            var responseHttpContext = await response.GetResposeValueAsync();
            var responseBody = await responseHttpContext.GetObjectFromBodyAsync<IEnumerable<ProductResponse>>();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            responseBody.Should().BeEquivalentTo(expectedResponse);
            responseBody.Should().NotBeNull();
            responseBody.Should().BeEmpty();
        }
    }
}