using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        public Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Client?> GetByReferralCodeAsync(string referralCode, CancellationToken ct = default);

        public IQueryable<T> GetAll();
        public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        public Task<T> FindAsync(Expression<Func<T, bool>> func);
    }
}
