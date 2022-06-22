namespace ProductsInventory.API.Application.Queries
{
    public class GetProductById
    {
        public static string Route => "/products/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [Authorize]
        [ClaimsAuthorize("Products", "Read")]
        public static async Task<IResult> Action(HttpContext context, IProductsRepository repository, 
                                                 ILogger<GetProductById> logger, 
                                                 IMapper mapper,
                                                 Guid id)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            logger.LogInformation($"User {userId} requested {Route} with productId: {id}");

            var product = await repository.GetByIdAsync(id);

            return product.Valid() ?
                logger.OkWithLog($"Product {id} was received by {userId}", mapper.Map<ProductResponse>(product)) :
                logger.NotFoundWithLog("Product not found", $", productId: {id}, userId: {userId}");
        }
    }
}
