using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Clients = new Repository<Client>(_context);
            WorkshopServices = new Repository<WorkshopService>(_context);
            ServiceCategories = new Repository<ServiceCategory>(_context);
            Specializations = new Repository<Specialization>(_context);
            Workshops = new WorkshopRepository(_context);
            Permissions = new PermissionRepository(_context);

        }
        public IRepository<Client> Clients { get; }
        public IRepository<WorkshopService> WorkshopServices { get; }
        public IRepository<ServiceCategory> ServiceCategories { get; }
        public IRepository<Specialization> Specializations { get; }
        public IWorkshopRepository Workshops { get; }
        public IPermissionRepository Permissions { get; }



        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }
        public void Dispose()
        {
            _context.Dispose();

        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
