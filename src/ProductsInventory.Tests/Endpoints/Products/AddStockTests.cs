namespace ProductsInventory.Tests.Endpoints.Products
{
    public class AddStockTests
    {
        [Trait("AddStock", "Products")]
        [Fact(DisplayName = "Add stock to a valid product")]
        public async Task AddStock_ValidProduct_ShouldAddTheCorrectQuantity()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<AddStock>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var quantity = 5;
            repository.GetByIdAsync(id).Returns(product);
            var initialQuantity = product.Quantity;
            repository.UnitOfWork.Commit().Returns(true);

            //Act
            var response = await AddStock.Action(repository, context, logger, id, quantity);

            //Assert
            product.Quantity.Should().Be(quantity + initialQuantity);
            response.Should().NotBeNull();
            response.GetResposeValueAsync().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Trait("AddStock", "Products")]
        [Fact(DisplayName = "Add invalid stock quantity to a valid product")]
        public async Task WithdrawFromStock_ValidProduct_ShouldReturnErrorBecauseQuantityCantBeLowerThanZero()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<AddStock>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var quantity = -1 ;
            repository.GetByIdAsync(id).Returns(product);

            //Act
            var action = async () => await AddStock.Action(repository, context, logger, id, quantity);

            //Assert
            await action.Should().ThrowAsync<InvalidQuantityException>();
        }

        [Trait("AddStock", "Products")]
        [Fact(DisplayName = "Add stock to a invalid product")]
        public async Task AddStock_InvalidProduct_ShouldReturnNotFound()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var id = Guid.NewGuid();
            var quantity = 5;
            var logger = Substitute.For<ILogger<AddStock>>();
            var responseMessage = "Product not found";
            var logComplementMessage = $", userId: {userId}, productId: {id}";
            var result = Results.NotFound(responseMessage);
            repository.GetByIdAsync(id).Returns(ProductsMock.GenerateNotExistingProduct());

            //Act
            var response = await AddStock.Action(repository, context, logger, id, quantity);

            //Assert
            response.Should().NotBeNull();
            response.GetResposeValueAsync().Result.Response.StatusCode.Should().Be(result.GetResposeValueAsync().Result.Response.StatusCode);
        }

        [Trait("AddStock", "Products")]
        [Fact(DisplayName = "Try add stock with service unavailable")]
        public async Task AddStock_ServiceUnavailable_ShouldReturnError()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<AddStock>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var quantity = 5;
            repository.GetByIdAsync(id).Returns(product);
            var initialQuantity = product.Quantity;
            repository.UnitOfWork.Commit().Returns(false);

            //Act
            var response = await AddStock.Action(repository, context, logger, id, quantity);

            //Assert
            product.Quantity.Should().Be(quantity + initialQuantity);
            response.Should().NotBeNull();
            response.GetResposeValueAsync().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
