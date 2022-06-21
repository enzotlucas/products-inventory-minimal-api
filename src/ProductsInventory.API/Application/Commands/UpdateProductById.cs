namespace ProductsInventory.API.Application.Commands
{
    public class UpdateProductById
    {
        public static string Route => "/products/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
        public static Delegate Handle => Action;

        [Authorize]
        [ClaimsAuthorize("Products", "Update")]
        public static async Task<IResult> Action(IProductsRepository repository, HttpContext context, ILogger<UpdateProductById> logger, Guid id, ProductViewModel dto)
        {
            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            logger.LogInformation($"User {userId} requested {Route} with productId: {id} and payload: {dto}");
            
            var product = await repository.GetByIdAsync(id);

            if (!product.Valid()) 
                return logger.NotFoundWithLog("Product not found", $", userId: {userId}, productId: {id}");

            product.Update(dto);

            await repository.UpdateAsync(product);

            if (!await repository.UnitOfWork.Commit())
                return logger.ProblemWithLog("Error on updating the product", $", userId: {userId}, productId: {id}, payload: {dto}");

            return logger.NoContentWithLog($"Product updated by {userId}, product: {product}");
        }
    }
}
