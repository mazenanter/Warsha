using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Features.CarCatalog.Commands.AddCarModel
{
    public class AddCarModelCommandHandler : IRequestHandler<AddCarModelCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddCarModelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddCarModelCommand request, CancellationToken cancellationToken)
        {
            var brand = await _unitOfWork.CarBrands.GetByIdAsync(request.BrandId);
            if (brand is null)
                return Result.Failure("Brand not found");

            var exists = await _unitOfWork.CarModels.FindAsync(
                m => m.CarBrandId == request.BrandId
                  && m.Name.ToLower() == request.Name.ToLower().Trim());

            if (exists != null)
                return Result.Failure(
                    $"Model '{request.Name}' already exists for {brand.Name}");

            var model = CarModel.Create(request.Name, request.BrandId);
            await _unitOfWork.CarModels.AddAsync(model);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success( "Model added successfully");
        }
    }
}
