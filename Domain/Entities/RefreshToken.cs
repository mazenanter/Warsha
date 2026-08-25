using Domain.Common;

namespace Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; private set; }
        public string Token { get; private set; } = default!;
        public DateTime ExpiresON { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresON;
        public DateTime? RevokedON { get; private set; }

        public bool IsActive => RevokedON == null && !IsExpired;

        protected RefreshToken() { }
        public static RefreshToken Create(string token, DateTime expiresOn)
        {
            return new RefreshToken
            {
                Token = token,
                ExpiresON = expiresOn,
                CreatedAt = DateTime.UtcNow
            };
        }
        public void Revoke()
        {
            if (IsExpired)
                throw new DomainException("Cannot revoke an expired token.");
            if (RevokedON != null)
                throw new DomainException("Token is already revoked.");
            RevokedON = DateTime.UtcNow;
        }
    }
}
