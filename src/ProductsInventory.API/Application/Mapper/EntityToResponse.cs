namespace ProductsInventory.API.Application.Mappings
{
    public class EntityToResponse : Profile
    {
        public EntityToResponse()
        {
            CreateMap<Product, ProductResponse>();
        }
    }
}
