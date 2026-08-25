using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Specialization.Commands.Delete
{
    public class InActiveSpecializationCommandHandler : IRequestHandler<InActiveSpecializationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InActiveSpecializationCommandHandler> _logger;

        public InActiveSpecializationCommandHandler(IUnitOfWork unitOfWork, ILogger<InActiveSpecializationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(InActiveSpecializationCommand request, CancellationToken cancellationToken)
        {
            var specialization = await _unitOfWork.Specializations.GetByIdAsync(request.Id);
            if (specialization is null)
            {
                _logger.LogWarning("Specialization with id: {Id} not found", request.Id);
                return Result.Failure("Specialization not found");
            }

            specialization.InActive();
            await _unitOfWork.Specializations.UpdateAsync(specialization);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("Specialization with id: {Id} DeActivated Now", request.Id);
            return Result.Failure("Specialization De Activated successfully");
        }
    }
}
