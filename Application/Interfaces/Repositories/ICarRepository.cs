using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
   public interface  ICarRepository : IRepository<Car>
    {
        Task<Car?> GetByModelAndYearAsync(int model, int year, CancellationToken ct = default);
        Task<IEnumerable<Car>> GetAllCars( CancellationToken ct = default);
        Task<Car?> GetCarById(int id, CancellationToken ct = default);
    }
}
