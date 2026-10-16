using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class LocalImageStorageService : IImageStorageService
    {
        private const long MaxFileSize = 5 * 1024 * 1024;

        private readonly IWebHostEnvironment _environment;

        public LocalImageStorageService(
            IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveWorkshopImageAsync(
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("Image file is required.");

            if (file.Length > MaxFileSize)
                throw new InvalidOperationException(
                    "Image size cannot exceed 5 MB.");

            var extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException(
                    "Only JPG, PNG, and WEBP images are allowed.");

            var allowedContentTypes = new[]
            {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

            if (!allowedContentTypes.Contains(file.ContentType))
                throw new InvalidOperationException(
                    "Invalid image content type.");

            var uploadsFolder = Path.Combine(
                _environment.WebRootPath ?? _environment.ContentRootPath,
                "uploads",
                "workshops");

            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return $"/uploads/workshops/{fileName}";
        }

        public Task DeleteImageAsync(
            string? imageUrl,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return Task.CompletedTask;

            var fileName = Path.GetFileName(imageUrl);

            if (string.IsNullOrWhiteSpace(fileName))
                return Task.CompletedTask;

            var uploadsFolder = Path.Combine(
                _environment.WebRootPath ?? _environment.ContentRootPath,
                "uploads",
                "workshops");

            var filePath = Path.Combine(uploadsFolder, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.CompletedTask;
        }
    }
}
