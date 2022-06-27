namespace ProductsInventory.API.Infrastructure.Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ProductsContext _context;

        public ProductsRepository(ProductsContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task CreateAsync(Product product)
            => await _context.Products.AddAsync(product);

        public async Task DeleteAsync(Product product)
            => await Task.Run(() => { _context.Products.Remove(product); });

        public async Task<IEnumerable<Product>> GetAllAsync(int page, int rows)
        {
            var query =
                @"SELECT *
                  FROM PRODUCTS
                  ORDER BY NAME
                  OFFSET (@page -1 ) * @rows ROWS FETCH NEXT @rows ROWS ONLY";

            var dbConnection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);

            return await dbConnection.QueryAsync<Product>(
               query,
               new
               {
                   page,
                   rows
               }
            );
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            return product ?? new Product(valid: false);
        }

        public async Task UpdateAsync(Product product)
            => await Task.Run(() => { _context.Products.Update(product); });

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
