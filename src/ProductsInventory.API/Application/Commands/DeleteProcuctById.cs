namespace ProductsInventory.API.Application.Commands
{
    public class DeleteProcuctById
    {
        public static string Route => "/products/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
        public static Delegate Handle => Action;

        [Authorize]
        [ClaimsAuthorize("Products", "Delete")]
        public static async Task<IResult> Action(HttpContext context, IProductsRepository repository, ILogger<DeleteProcuctById> logger, Guid id)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            logger.LogInformation($"User {userId} requested {Route} with productId: {id}");

            var product = await repository.GetByIdAsync(id);

            if (!product.Valid()) 
                return logger.NotFoundWithLog("Product not found", $", userId: {userId}, productId: {id}");

            await repository.DeleteAsync(product);

            if (!await repository.UnitOfWork.Commit())
                return logger.ProblemWithLog("Error on deleting the product", $", userId: {userId}, productId: {id}");

            return logger.NoContentWithLog($"Product deleted, productId: {id}, userId: {userId}");
        }
    }
}
