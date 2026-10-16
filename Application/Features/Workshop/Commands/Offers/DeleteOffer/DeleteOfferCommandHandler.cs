using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Offers.DeleteOffer
{
    public class DeleteOfferCommandHandler
    : IRequestHandler<DeleteOfferCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public DeleteOfferCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(
            DeleteOfferCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            var workshop = await _unitOfWork.Workshops
                .GetByUserIdAsync(userId, cancellationToken);

            if (workshop is null)
                return Result.Failure("Workshop not found.");

            var offer = await _unitOfWork.Offers
                .GetByIdAsync(request.OfferId, cancellationToken);

            if (offer is null || offer.WorkshopId != workshop.Id)
                return Result.Failure("Offer not found.");

             await _unitOfWork.Offers.DeleteAsync(offer);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Offer deleted successfully.");
        }
    }
}
