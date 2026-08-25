using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        public Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public IQueryable<T> GetAll();
        public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        public Task<T> FindAsync(Expression<Func<T, bool>> func);
    }
}
