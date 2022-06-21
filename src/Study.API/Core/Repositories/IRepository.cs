namespace Study.API.Core.Repositories
{
    public interface IRepository<T> : IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}