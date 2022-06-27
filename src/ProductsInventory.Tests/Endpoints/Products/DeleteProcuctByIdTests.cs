namespace ProductsInventory.Tests.Endpoints.Products
{
    public class DeleteProcuctByIdTests
    {
        [Trait("DeleteProcuctById", "Products")]
        [Fact(DisplayName = "Delete existing product")]
        public async Task DeleteProcuctById_ValidProduct_ShouldDeleteFromDatabase()
        {
            //Arrange
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var repository = Substitute.For<IProductsRepository>();
            var logger = Substitute.For<ILogger<DeleteProcuctById>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;

            repository.GetByIdAsync(id).Returns(product);
            repository.UnitOfWork.Commit().Returns(true);

            //Act
            var response = await DeleteProcuctById.Action(context, repository, logger, id);

            //Assert
            response.Should().NotBeNull();
            response.GetResposeValueAsync().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Trait("DeleteProcuctById", "Products")]
        [Fact(DisplayName = "Try delete unexistent product")]
        public async Task DeleteProcuctById_InvalidProduct_ShouldDeleteFromDatabase()
        {
            //Arrange
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var repository = Substitute.For<IProductsRepository>();
            var logger = Substitute.For<ILogger<DeleteProcuctById>>();
            var product = ProductsMock.GenerateNotExistingProduct();
            var id = product.Id;

            repository.GetByIdAsync(id).Returns(product);

            //Act
            var response = await DeleteProcuctById.Action(context, repository, logger, id);

            //Assert
            response.Should().NotBeNull();
            response.GetResposeValueAsync().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Trait("DeleteProcuctById", "Products")]
        [Fact(DisplayName = "Try delete product with service unavailable")]
        public async Task DeleteProcuctById_ServiceUnavailable_ShouldReturnError()
        {
            //Arrange
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var repository = Substitute.For<IProductsRepository>();
            var logger = Substitute.For<ILogger<DeleteProcuctById>>();
            var product = ProductsMock.GenerateValidProduct();
            var id = product.Id;

            repository.GetByIdAsync(id).Returns(product);
            repository.UnitOfWork.Commit().Returns(false);

            //Act
            var response = await DeleteProcuctById.Action(context, repository, logger, id);

            //Assert
            response.Should().NotBeNull();
            response.GetResposeValueAsync().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
