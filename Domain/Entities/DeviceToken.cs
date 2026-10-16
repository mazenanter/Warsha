using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DeviceToken : BaseEntity
    {
        public int UserId { get; private set; }
        public string Token { get; private set; } = default!;
        public string Platform { get; private set; } = default!;
        public string DeviceId { get; private set; } = default!;
        public DateTime LastUsed { get; private set; }

        protected DeviceToken() { }

        public static DeviceToken Create(int userId, string token, string platform, string deviceId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("Device token is required");
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new DomainException("Device ID is required");
            return new DeviceToken
            {
                UserId = userId,
                DeviceId = deviceId,
                Token = token,
                Platform = platform.ToLowerInvariant(),
                LastUsed = DateTime.UtcNow
            };
        }

        public void Update(
       string newToken,
       string platform)
        {
            if (string.IsNullOrWhiteSpace(newToken))
                throw new DomainException("Device token is required");

            Token = newToken;
            Platform = platform.ToLowerInvariant();
            LastUsed = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
