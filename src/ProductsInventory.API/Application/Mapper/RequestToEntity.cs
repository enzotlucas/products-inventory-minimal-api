namespace ProductsInventory.API.Application.Mappings
{
    public class RequestToEntity : Profile
    {
        public RequestToEntity()
        {
            CreateMap<ProductRequest, Product>()
                .ForCtorParam("name", options => options.MapFrom(request => request.Name))
                .ForCtorParam("quantity", options => options.MapFrom(request => request.Quantity))
                .ForCtorParam("price", options => options.MapFrom(request => request.Price))
                .ForCtorParam("cost", options => options.MapFrom(request => request.Cost))
                .ForCtorParam("enabled", options => options.MapFrom(request => request.Enabled))
                .ForCtorParam("validator", options => options.MapFrom(request => request.Validator)); 
        }
    }
}
