namespace ProductsInventory.API.Core.Repositories
{
    public interface IRepository<T> : IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}