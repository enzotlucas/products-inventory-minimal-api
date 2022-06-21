namespace ProductsInventory.API.Core.DomainObjects
{
    public interface IDefinition
    {
        void DefineActions(WebApplication app);
        void DefineServices(WebApplicationBuilder builder);
    }
}
