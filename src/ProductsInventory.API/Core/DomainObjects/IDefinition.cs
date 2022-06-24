namespace ProductsInventory.API.Core.DomainObjects
{
    public interface IDefinition
    {
        int OrderPriority { get; }

        void DefineActions(WebApplication app);
        void DefineServices(WebApplicationBuilder builder);
    }
}
