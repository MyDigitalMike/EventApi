using EventApi.Models;
using System.Linq.Expressions;

namespace EventApi.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAsync(
    Expression<Func<T, bool>> predicate,
    params Expression<Func<T, object>>[] includeProperties);
        Task AddAsync(T entity);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}