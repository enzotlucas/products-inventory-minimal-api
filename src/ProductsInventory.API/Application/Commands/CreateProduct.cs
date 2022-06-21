namespace ProductsInventory.API.Application.Commands
{
    public class CreateProduct
    {
        public static string Route => "/products";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [Authorize]
        [ClaimsAuthorize("Products", "Create")]
        public static async Task<IResult> Action(HttpContext context, IProductsRepository repository, ILogger<CreateProduct> logger, ProductViewModel dto)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            logger.LogInformation(string.Format("User {0} requested {1} with payload: {2}", userId, Route, dto.ToString()));

            var product = dto.ToEntity();

            await repository.CreateAsync(product);

            if (!await repository.UnitOfWork.Commit())
                return logger.ProblemWithLog("Error on creating the product", $", userId: {userId}, payload: {dto}");

            return logger.OkWithLog($"Product created by {userId}, product id: {product.Id}", new ProductViewModel(product));
        }
    }
}
