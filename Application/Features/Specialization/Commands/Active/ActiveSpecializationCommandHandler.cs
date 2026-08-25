using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Specialization.Commands.Active
{
    public class ActiveSpecializationCommandHandler : IRequestHandler<ActiveSpecializationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActiveSpecializationCommandHandler> _logger;

        public ActiveSpecializationCommandHandler(IUnitOfWork unitOfWork, ILogger<ActiveSpecializationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(ActiveSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.Specializations.GetByIdAsync(request.Id);
            if (specialization is null)
            {
                _logger.LogWarning("Specialization with id: {Id} not found", request.Id);
                return Result.Failure("Specialization not found");
            }

            specialization.Active();
            await _unitOfWork.Specializations.UpdateAsync(specialization);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("Specialization with id: {Id} Activated Now", request.Id);
            return Result.Failure("Specialization Activated successfully");
        }
    }
}
