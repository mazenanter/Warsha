namespace Application.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        int? WorkshopId { get; }
        int? ClientId { get; }
        string Role { get; }
    }
}
