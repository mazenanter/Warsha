using Application.Features.Client.Cars.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CarCatalog.Queries.GetCarModels
{
    public class GetCarModelsQueryHandler : IRequestHandler<GetCarModelsQuery, Result<List<CarModelResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCarModelsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<CarModelResponseDto>>> Handle(GetCarModelsQuery request, CancellationToken cancellationToken)
        {
            var carModels = await _unitOfWork.CarModels.GetAll().Where(cm => cm.CarBrandId == request.CarBrandId).Select(cm => new CarModelResponseDto
            {
                Id = cm.Id,
                Name = cm.Name
            }).ToListAsync();
            return Result<List<CarModelResponseDto>>.Success(carModels, "Car models retrieved successfully.");
        }
    }
}
