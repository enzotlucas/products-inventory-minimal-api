namespace ProductsInventory.Tests.Endpoints.Products
{
    public class GetProductByIdTests
    {
        [Trait("GetProductById", "Products")]
        [Fact(DisplayName = "Get the product from database")]
        public async Task GetProductById_ProductExists_ShouldReturnProduct()
        {
            //Arrange
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var repository = Substitute.For<IProductsRepository>();
            var logger = Substitute.For<ILogger<GetProductById>>();
            var mapper = Substitute.For<IMapper>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;

            repository.GetByIdAsync(id).Returns(product);

            var expectedResponse = ProductsMock.GenerateValidProductResponse();
            mapper.Map<ProductResponse>(product).Returns(expectedResponse);


            //Act
            var response = await GetProductById.Action(context, repository, logger, mapper, id);
            var responseHttpContext = await response.GetResposeValueAsync();
            var responseBody = await responseHttpContext.GetObjectFromBodyAsync<ProductResponse>();

            //Assert
            response.Should().NotBeNull();
            responseHttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            responseBody.Should().BeEquivalentTo(expectedResponse);
            responseBody.Should().NotBeNull();
        }

        [Trait("GetProductById", "Products")]
        [Fact(DisplayName = "Try get products that don't exists")]
        public async Task GetProductById_ProductDontExists_ShouldReturnNotFound()
        {
            //Arrange
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var repository = Substitute.For<IProductsRepository>();
            var logger = Substitute.For<ILogger<GetProductById>>();
            var mapper = Substitute.For<IMapper>();
            var product = ProductsMock.GenerateNotExistingProduct();
            var id = product.Id;

            repository.GetByIdAsync(id).Returns(product);

            //Act
            var response = await GetProductById.Action(context, repository, logger, mapper, id);

            //Assert
            response.Should().NotBeNull();
            response.GetResposeValueAsync().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
