namespace Application.Interfaces
{
    public interface IIdentityService
    {
        Task<bool> IsActive(int userId);
    }
}
