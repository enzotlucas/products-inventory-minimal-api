using System.Security.Claims;

namespace ProductsInventory.API.Application.Queries
{
    public class GetAllProducts
    {
        public static string Route => "/products";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        //[Authorize(Policy = "CanReadProducts")]
        [Authorize]
        //[ClaimsAuthorize("Products","Read")]
        public static async Task<IResult> Action(IProductsRepository repository, HttpContext context, ILogger<GetAllProducts> logger, int page, int rows)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            logger.LogInformation($"User {userId} requested {Route},page number: {page} and ammount of rows: {rows}");

            var products = await repository.GetAllAsync(page, rows);

            return logger.OkWithLog($"Get products was requested by {userId}, results count: {products.Count()}", products);
        }
    }
}
