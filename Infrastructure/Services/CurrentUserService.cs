using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _accessor;
        public CurrentUserService(IHttpContextAccessor accessor)
            => _accessor = accessor;
        public int UserId =>
        int.Parse(_accessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

        public int? WorkshopId
        {
            get
            {
                var val = _accessor.HttpContext!.User.FindFirstValue("workshopId");
                return val == null ? null : int.Parse(val);
            }
        }

        public int? ClientId
        {
            get
            {
                var val = _accessor.HttpContext!.User.FindFirstValue("clientId");
                return val == null ? null : int.Parse(val);
            }
        }

        public string Role =>
            _accessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role)!;
    }
}
