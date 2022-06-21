namespace Study.API.Core.Repositories
{
    public interface IProductsRepository : IRepository<Product>
    {
        Task CreateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync(int page, int rows);
        Task<Product> GetByIdAsync(Guid id);
        Task UpdateAsync(Product product);
    }
}
