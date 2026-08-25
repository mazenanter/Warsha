using Domain.Entities;

namespace Application.Interfaces
{
    public interface IJwtService
    {
        public string GenerateAccessToken(int userId, string email, IList<string> roles, 
        int? clientId =null, int? workshopId =null);
        public RefreshToken GenerateRefreshToken();
    }
}
