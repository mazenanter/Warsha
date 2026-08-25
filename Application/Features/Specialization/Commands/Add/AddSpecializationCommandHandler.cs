using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Specialization.Commands.Add
{
    public class AddSpecializationCommandHandler : IRequestHandler<AddSpecializationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddSpecializationCommandHandler> _logger;
       
        public async Task<Result> Handle(AddSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specializatoin = await _unitOfWork.Specializations.FindAsync(x => x.Name.ToLower() == request.Name.ToLower());
            if(specializatoin != null)
            {
                _logger.LogWarning("Specialization already exist ");
                return Result.Failure("Specialization already exist.");
            }
            var newSpecialization = Domain.Entities.Specialization.Create(request.Name, request.Icon);
            _logger.LogInformation("Specialization {Name} added successfully", request.Name);
            await _unitOfWork.Specializations.AddAsync(newSpecialization);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success("Specialization added successfully.");
        }
    }
}
