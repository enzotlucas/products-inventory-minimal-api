namespace ProductsInventory.API.Application.Commands
{
    public class CreateProduct
    {
        public static string Route => "/api/products";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "CanCreateProduct")]
        internal static async Task<IResult> Action(HttpContext context, 
                                                 IProductsRepository repository, 
                                                 ILogger<CreateProduct> logger, 
                                                 IMapper mapper,
                                                 IValidator<Product> validator,
                                                 ProductRequest dto)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            dto.Validator = validator;

            var product = mapper.Map<Product>(dto);

            await repository.CreateAsync(product);

            if (!await repository.UnitOfWork.Commit())
                return logger.ProblemWithLog("Error on creating the product", $", userId: {userId}, payload: {dto}");

            return logger.OkWithLog($"Product created by {userId}, product id: {product.Id}", mapper.Map<ProductResponse>(product));
        }
    }
}
