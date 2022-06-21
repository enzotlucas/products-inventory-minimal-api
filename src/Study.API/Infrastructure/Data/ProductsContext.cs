namespace Study.API.Infrastructure.Data
{
    public class ProductsContext : DbContext, IUnitOfWork
    {
        public ProductsContext(DbContextOptions<ProductsContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(p => 
            {
                p.HasKey(product => product.Id);

                p.ToTable("Products");
            });

            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> Commit()
        {
            foreach (var entry in ChangeTracker.Entries()
                            .Where(entry => entry.Entity.GetType().GetProperty("CreatedAt") != null))
            {
                if (entry.State == EntityState.Added)
                    entry.Property("CreatedAt").CurrentValue = DateTime.Now;

                if (entry.State == EntityState.Modified)
                    entry.Property("CreatedAt").IsModified = false;
            }

            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("UpdatedAt") != null))
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    entry.Property("UpdatedAt").CurrentValue = DateTime.Now;

            return await base.SaveChangesAsync() > 0;
        }
    }
}
