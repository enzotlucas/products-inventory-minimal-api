namespace ProductsInventory.Tests.Endpoints.Products
{
    public class WithdrawFromStockTests
    {
        [Trait("WithdrawFromStock", "Products")]
        [Fact(DisplayName = "Withdraw from stock of a valid product")]
        public async Task WithdrawFromStock_ValidProduct_ShouldWithDrawTheCorrectQuantity()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<WithdrawFromStock>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var quantity = 2;
            repository.GetByIdAsync(id).Returns(product);
            var initialQuantity = product.Quantity;
            repository.UnitOfWork.Commit().Returns(true);

            //Act
            var response = await WithdrawFromStock.Action(repository, context, logger, id, quantity);

            //Assert
            product.Quantity.Should().Be(initialQuantity - quantity);
            response.GetResposeValue().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Trait("WithdrawFromStock", "Products")]
        [Fact(DisplayName = "Withdraw from stock of a valid product with quantity lower than zero")]
        public async Task WithdrawFromStock_ValidProduct_ShouldReturnErrorBecauseQuantityCantBeLowerThanZero()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<WithdrawFromStock>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var quantity = 100;
            repository.GetByIdAsync(id).Returns(product);

            //Act
            var action = async () => await WithdrawFromStock.Action(repository, context, logger, id, quantity);

            //Assert
            await action.Should().ThrowAsync<InvalidQuantityException>();
        }

        [Trait("WithdrawFromStock", "Products")]
        [Fact(DisplayName = "Withdraw quantity lower than one from stock of a valid product")]
        public async Task WithdrawFromStock_ValidProduct_ShouldReturnErrorBecauseQuantityParameterCantBeLowerThanOne()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<WithdrawFromStock>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var quantity = 0;
            repository.GetByIdAsync(id).Returns(product);

            //Act
            var action = async () => await WithdrawFromStock.Action(repository, context, logger, id, quantity);

            //Assert
            await action.Should().ThrowAsync<InvalidQuantityException>();
        }

        [Trait("WithdrawFromStock", "Products")]
        [Fact(DisplayName = "Withdraw from stock of a valid product with 0 stock quantity")]
        public async Task WithdrawFromStock_ValidProductWithZeroQuantity_ShouldReturnErrorBecauseCantWithdraw()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<WithdrawFromStock>>();
            var product = ProductsMock.GenerateValidProduct();
            product.WithdrawFromStock(product.Quantity);
            var id = product.Id;
            var quantity = 5;
            repository.GetByIdAsync(id).Returns(product);

            //Act
            var action = async () => await WithdrawFromStock.Action(repository, context, logger, id, quantity);

            //Assert
            await action.Should().ThrowAsync<InvalidQuantityException>();
        }

        [Trait("WithdrawFromStock", "Products")]
        [Fact(DisplayName = "Withdraw from stock of a unexistent product")]
        public async Task WithdrawFromStock_UnexistentProduct_ShouldReturnNotFound()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var id = Guid.NewGuid();
            var quantity = 5;
            var logger = Substitute.For<ILogger<WithdrawFromStock>>();
            var responseMessage = "Product not found";
            var logComplementMessage = $", userId: {userId}, productId: {id}";
            var result = Results.NotFound(responseMessage);
            repository.GetByIdAsync(id).Returns(ProductsMock.GenerateNotExistingProduct());

            //Act
            var response = await WithdrawFromStock.Action(repository, context, logger, id, quantity);

            //Assert
            response.GetResposeValue().Result.Response.StatusCode.Should().Be(result.GetResposeValue().Result.Response.StatusCode);
        }

        [Trait("WithdrawFromStock", "Products")]
        [Fact(DisplayName = "Try withdraw from stock with service unavailable")]
        public async Task AddStock_ServiceUnavailable_ShouldReturnError()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<WithdrawFromStock>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var quantity = 2;
            repository.GetByIdAsync(id).Returns(product);
            var initialQuantity = product.Quantity;
            repository.UnitOfWork.Commit().Returns(false);

            //Act
            var response = await WithdrawFromStock.Action(repository, context, logger, id, quantity);

            //Assert
            product.Quantity.Should().Be(initialQuantity - quantity);
            response.GetResposeValue().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
