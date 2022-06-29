namespace ProductsInventory.Tests.Endpoints.Products
{
    public class UpdateProductByIdTests
    {
        [Trait("UpdateProductById", "Products")]
        [Fact(DisplayName = "Update a valid product")]
        public async Task UpdateProductById_ValidProduct_ShouldUpdateSuccessfully()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<UpdateProductById>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var dto = ProductsMock.GenerateValidProductRequest();

            repository.GetByIdAsync(id).Returns(product);
            repository.UnitOfWork.Commit().Returns(true);

            //Act
            var response = await UpdateProductById.Action(repository, context, logger, id, dto);

            //Assert
            response.Should().NotBeNull();
            response.GetResposeHttpContextAsync().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Trait("UpdateProductById", "Products")]
        [Fact(DisplayName = "Try update a unexistent product")]
        public async Task UpdateProductById_InvalidProduct_ShouldReturnNotFound()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<UpdateProductById>>();
            var product = ProductsMock.GenerateNotExistingProduct();
            var id = product.Id;
            var dto = ProductsMock.GenerateValidProductRequest();

            repository.GetByIdAsync(id).Returns(product);

            //Act
            var response = await UpdateProductById.Action(repository, context, logger, id, dto);

            //Assert
            response.Should().NotBeNull();
            response.GetResposeHttpContextAsync().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Trait("UpdateProductById", "Products")]
        [Fact(DisplayName = "Try update product with invalid name")]
        public async Task UpdateProductById_InvalidName_ShouldReturnException()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<UpdateProductById>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var dto = ProductsMock.GenerateInvalidProductRequest(invalidName:true);

            repository.GetByIdAsync(id).Returns(product);

            //Act
            var action = async () => await UpdateProductById.Action(repository, context, logger, id, dto);

            //Assert
            await action.Should().ThrowAsync<InvalidNameException>();
        }

        [Trait("UpdateProductById", "Products")]
        [Fact(DisplayName = "Try update product with invalid quantity")]
        public async Task UpdateProductById_InvalidQuantity_ShouldReturnException()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<UpdateProductById>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var dto = ProductsMock.GenerateInvalidProductRequest(invalidQuantity: true);

            repository.GetByIdAsync(id).Returns(product);

            //Act
            var action = async () => await UpdateProductById.Action(repository, context, logger, id, dto);

            //Assert
            await action.Should().ThrowAsync<InvalidQuantityException>();
        }

        [Trait("UpdateProductById", "Products")]
        [Fact(DisplayName = "Try update product with invalid cost")]
        public async Task UpdateProductById_InvalidCost_ShouldReturnException()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<UpdateProductById>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var dto = ProductsMock.GenerateInvalidProductRequest(invalidCost: true);

            repository.GetByIdAsync(id).Returns(product);

            //Act
            var action = async () => await UpdateProductById.Action(repository, context, logger, id, dto);

            //Assert
            await action.Should().ThrowAsync<InvalidCostException>();
        }

        [Trait("UpdateProductById", "Products")]
        [Fact(DisplayName = "Try update product with invalid price")]
        public async Task UpdateProductById_InvalidPrice_ShouldReturnException()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<UpdateProductById>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var dto = ProductsMock.GenerateInvalidProductRequest(invalidPrice: true);

            repository.GetByIdAsync(id).Returns(product);

            //Act
            var action = async () => await UpdateProductById.Action(repository, context, logger, id, dto);

            //Assert
            await action.Should().ThrowAsync<InvalidPriceException>();
        }

        [Trait("UpdateProductById", "Products")]
        [Fact(DisplayName = "Try update a product with service unavailable")]
        public async Task UpdateProductById_ServiceUnavailable_ShouldReturnError()
        {
            //Arrange
            var repository = Substitute.For<IProductsRepository>();
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var logger = Substitute.For<ILogger<UpdateProductById>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;
            var dto = ProductsMock.GenerateValidProductRequest();

            repository.GetByIdAsync(id).Returns(product);
            repository.UnitOfWork.Commit().Returns(false);

            //Act
            var response = await UpdateProductById.Action(repository, context, logger, id, dto);

            //Assert
            response.Should().NotBeNull();
            response.GetResposeHttpContextAsync().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
