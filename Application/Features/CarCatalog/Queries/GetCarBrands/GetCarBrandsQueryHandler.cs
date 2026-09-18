using Application.Features.Client.Cars.DTOs;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace Application.Features.CarCatalog.Queries.GetCarBrands
{
    public class GetCarBrandsQueryHandler : IRequestHandler<GetCarBrandsQuery, Result<List<CarBrandsResponseDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCarBrandsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<CarBrandsResponseDto>>> Handle(GetCarBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await _unitOfWork.CarBrands.GetAll().Select(x => new CarBrandsResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Icon = x.Icon
            }).ToListAsync();

            return Result<List<CarBrandsResponseDto>>.Success(brands, "Car brands retrieved successfully.");
        }
    }
}
