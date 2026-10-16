using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Workshop.Commands.UpdateWorkshopImage
{
    public class UpdateWorkshopImageCommandHandler
    : IRequestHandler<UpdateWorkshopImageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageStorageService _imageStorageService;
        private readonly ILogger<UpdateWorkshopImageCommandHandler> _logger;

        public UpdateWorkshopImageCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IImageStorageService imageStorageService,
            ILogger<UpdateWorkshopImageCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _imageStorageService = imageStorageService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(
            UpdateWorkshopImageCommand request,
            CancellationToken cancellationToken)
        {
            var workshop = await _unitOfWork.Workshops
                .GetByUserIdAsync(
                    _currentUserService.UserId,
                    cancellationToken);

            if (workshop is null)
                return Result<string>.Failure(
                    "Workshop profile not found.");

            var oldImageUrl = workshop.ImageUrl;

            var newImageUrl =
                await _imageStorageService.SaveWorkshopImageAsync(
                    request.Image,
                    cancellationToken);

            try
            {
                workshop.UpdateImage(newImageUrl);

                await _unitOfWork.Workshops.UpdateAsync(workshop);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                try
                {
                    await _imageStorageService.DeleteImageAsync(
                        newImageUrl,
                        CancellationToken.None);
                }
                catch (Exception cleanupException)
                {
                    _logger.LogError(
                        cleanupException,
                        "Failed to clean up new workshop image {ImageUrl}",
                        newImageUrl);
                }

                throw;
            }

            if (!string.IsNullOrWhiteSpace(oldImageUrl))
            {
                try
                {
                    await _imageStorageService.DeleteImageAsync(
                        oldImageUrl,
                        cancellationToken);
                }
                catch (Exception cleanupException)
                {
                    _logger.LogWarning(
                        cleanupException,
                        "Failed to delete old workshop image {ImageUrl}",
                        oldImageUrl);
                }
            }

            _logger.LogInformation(
                "Workshop image updated for workshop {WorkshopId}",
                workshop.Id);

            return Result<string>.Success(
                newImageUrl,
                "Workshop image updated successfully.");
        }
    }
}
