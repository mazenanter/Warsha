using FluentValidation;

namespace Application.Features.Workshop.Commands.UpdateWorkshopImage
{
    public class UpdateWorkshopImageCommandValidator
     : AbstractValidator<UpdateWorkshopImageCommand>
    {
        private static readonly string[] AllowedContentTypes =
        [
            "image/jpeg",
        "image/png",
        "image/webp"
        ];

        private static readonly string[] AllowedExtensions =
        [
            ".jpg",
        ".jpeg",
        ".png",
        ".webp"
        ];

        public UpdateWorkshopImageCommandValidator()
        {
            RuleFor(x => x.Image)
                .NotNull()
                .WithMessage("Image is required.")
                .Must(file => file is not null && file.Length > 0)
                .WithMessage("Image cannot be empty.")
                .Must(file => file is not null &&
                              file.Length <= 5 * 1024 * 1024)
                .WithMessage("Image size cannot exceed 5 MB.")
                .Must(file => file is not null &&
                              AllowedContentTypes.Contains(file.ContentType))
                .WithMessage("Only JPG, PNG, and WEBP images are allowed.")
                .Must(file => file is not null &&
                              AllowedExtensions.Contains(
                                  Path.GetExtension(file.FileName)
                                      .ToLowerInvariant()))
                .WithMessage("Invalid image extension.");
        }
    }
}
