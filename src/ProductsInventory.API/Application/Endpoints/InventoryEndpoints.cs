namespace ProductsInventory.API.Application.Endpoints
{
    public class InventoryEndpoints : IDefinition
    {
        public int OrderPriority => 5;

        public void DefineActions(WebApplication app)
        {
            app.MapMethods(CreateProduct.Route, CreateProduct.Methods, CreateProduct.Handle)
               .WithTags("Products")
               .ProducesValidationProblem()
               .Produces<ProductResponse>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest);

            app.MapMethods(DeleteProcuctById.Route, DeleteProcuctById.Methods, DeleteProcuctById.Handle)
               .WithTags("Products")
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status404NotFound);

            app.MapMethods(UpdateProductById.Route, UpdateProductById.Methods, UpdateProductById.Handle)
               .WithTags("Products")
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status404NotFound);

            app.MapMethods(AddStock.Route, AddStock.Methods, AddStock.Handle)
               .WithTags("Products")
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status404NotFound);

            app.MapMethods(WithdrawFromStock.Route, WithdrawFromStock.Methods, WithdrawFromStock.Handle)
               .WithTags("Products")
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status404NotFound);

            app.MapMethods(GetAllProducts.Route, GetAllProducts.Methods, GetAllProducts.Handle)
               .WithTags("Products")
               .Produces(StatusCodes.Status200OK);

            app.MapMethods(GetProductById.Route, GetProductById.Methods, GetProductById.Handle)
               .WithTags("Products")
               .Produces(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound);
        }

        public void DefineServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
        }
    }
}
