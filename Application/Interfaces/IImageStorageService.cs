using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> SaveWorkshopImageAsync(
            IFormFile file,
            CancellationToken cancellationToken = default);

        Task DeleteImageAsync(
            string? imageUrl,
            CancellationToken cancellationToken = default);
    }
}
