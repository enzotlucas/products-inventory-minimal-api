namespace ProductsInventory.API.Application.Commands
{
    public class AddStock
    {
        public static string Route => "/api/products/{id:guid}/addStock";
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy ="CanUpdateProduct")]
        internal static async Task<IResult> Action(IProductsRepository repository, 
                                                 HttpContext context, 
                                                 ILogger<AddStock> logger, 
                                                 Guid id, 
                                                 int quantity)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var product = await repository.GetByIdAsync(id);

            if (!product.Valid())
                return logger.NotFoundWithLog("Product not found", $", userId: {userId}, productId: {id}");

            product.AddStock(quantity);

            await repository.UpdateAsync(product);

            if (!await repository.UnitOfWork.Commit())
                return logger.ProblemWithLog("Error on updating the product", $", userId: {userId}, productId: {id}");

            return logger.NoContentWithLog($"Product stock updated by {userId}, productId: {id}, current stock: {product.Quantity}");
        }
    }
}
