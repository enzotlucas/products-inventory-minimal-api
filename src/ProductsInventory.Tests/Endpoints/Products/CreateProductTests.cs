using AutoMapper;

namespace ProductsInventory.Tests.Endpoints.Products
{
    public class CreateProductTests
    {
        [Trait("CreateProduct", "Products")]
        [Fact(DisplayName = "Create product with a valid product")]
        public async Task CreateProduct_ValidProduct_ShouldReturnSuccess()
        {
            //Arrange
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var repository = Substitute.For<IProductsRepository>();
            repository.UnitOfWork.Commit().Returns(true);
            var logger = Substitute.For<ILogger<CreateProduct>>();
            var mapper = Substitute.For<IMapper>();
            var validator = Substitute.For<IValidator<Product>>();
            var productDto = ProductsMock.GenerateValidProductRequest();
            var product = ProductsMock.GenerateValidProduct();
            mapper.Map<Product>(productDto).Returns(product);

            //Act
            var response = await CreateProduct.Action(context, repository, logger, mapper, validator, productDto);

            //Assert
            response.GetResposeValue().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Trait("CreateProduct", "Products")]
        [Fact(DisplayName = "Create product with a invalid product")]
        public async Task CreateProduct_InvalidProduct_ShouldReturnBadRequest()
        {
            //Arrange
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var repository = Substitute.For<IProductsRepository>();
            var logger = Substitute.For<ILogger<CreateProduct>>();
            var mapper = Substitute.For<IMapper>();
            var validator = Substitute.For<IValidator<Product>>();
            var productDto = ProductsMock.GenerateInvalidProductRequest();
            mapper.When(x => x.Map<Product>(productDto)).Throw(new InvalidProductException());

            //Act
            var action = async () => await CreateProduct.Action(context, repository, logger, mapper, validator, productDto);

            //Assert
            await action.Should().ThrowAsync<InvalidProductException>();
        }

        [Trait("CreateProduct", "Products")]
        [Fact(DisplayName = "Try create product with service unavailable")]
        public async Task CreateProduct_ServiceUnavailable_ShouldReturnError()
        {
            //Arrange
            var context = HttpContextMock.GenerateAuthenticateduserHttpContext(UserType.ADMINISTRATOR);
            var repository = Substitute.For<IProductsRepository>();
            var logger = Substitute.For<ILogger<CreateProduct>>();
            var mapper = Substitute.For<IMapper>();
            var validator = Substitute.For<IValidator<Product>>();
            var productDto = ProductsMock.GenerateValidProductRequest();
            var product = ProductsMock.GenerateValidProduct();
            mapper.Map<Product>(productDto).Returns(product);
            repository.UnitOfWork.Commit().Returns(false);

            //Act
            var response = await CreateProduct.Action(context, repository, logger, mapper, validator, productDto);

            //Assert
            response.GetResposeValue().Result.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}