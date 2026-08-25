using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        public IRepository<Client> Clients { get;}
        public IWorkshopRepository Workshops { get;}
        public IRepository<WorkshopService> WorkshopServices { get;}
        public IRepository<ServiceCategory> ServiceCategories { get;}
        public IRepository<Specialization> Specializations { get;}
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
