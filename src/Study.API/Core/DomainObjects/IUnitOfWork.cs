namespace Study.API.Core.DomainObjects
{
    public interface IUnitOfWork
    {
         Task<bool> Commit();
    }
}