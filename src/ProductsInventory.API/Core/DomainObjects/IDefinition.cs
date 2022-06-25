namespace ProductsInventory.API.Core.DomainObjects
{
    public interface IDefinition
    {
        int ConfigurationOrder { get; }

        void DefineActions(WebApplication app);
        void DefineServices(WebApplicationBuilder builder);
    }
}
