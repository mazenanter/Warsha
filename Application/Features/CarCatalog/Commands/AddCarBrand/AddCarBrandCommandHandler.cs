using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Features.CarCatalog.Commands.AddCarBrand
{
    public class AddCarBrandCommandHandler : IRequestHandler<AddCarBrandCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddCarBrandCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddCarBrandCommand request, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.CarBrands
           .FindAsync(b => b.Name.ToLower() == request.Name.ToLower().Trim());

            if (exists != null)
                return Result.Failure($"Brand '{request.Name}' already exists");

            var brand = CarBrand.Create(request.Name, request.Icon);
            await _unitOfWork.CarBrands.AddAsync(brand, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success( "Brand added successfully");
        }
    }
}
