namespace ProductsInventory.API.Application.Queries
{
    public class GetAllProducts
    {
        public static string Route => "/api/products";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "CanReadProduct")]
        internal static async Task<IResult> Action(IProductsRepository repository, 
                                                 HttpContext context, 
                                                 ILogger<GetAllProducts> logger, 
                                                 IMapper mapper,
                                                 int page, 
                                                 int rows)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var products = await repository.GetAllAsync(page, rows);

            return logger.OkWithLog($"Get products was requested by {userId}, results count: {products.Count()}", 
                                    mapper.Map<IEnumerable<ProductResponse>>(products));
        }
    }
}
