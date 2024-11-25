using EventApi.Models;

namespace EventApi.Repositories
{
    public interface IUnitOfWork: IDisposable
    {
        IRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveChangesAsync();
    }
}
