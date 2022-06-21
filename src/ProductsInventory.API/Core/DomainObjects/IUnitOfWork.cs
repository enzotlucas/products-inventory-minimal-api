namespace ProductsInventory.API.Core.DomainObjects
{
    public interface IUnitOfWork
    {
         Task<bool> Commit();
    }
}