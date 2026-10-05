namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(
            int userId, string title, string body,
            Dictionary<string, string>? data = null,
            CancellationToken ct = default);
    }
}
