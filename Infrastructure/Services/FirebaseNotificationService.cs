using Application.Interfaces;
using Domain.Entities;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using FirebaseNotification = FirebaseAdmin.Messaging.Notification;

namespace Infrastructure.Services;

public class FirebaseNotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FirebaseNotificationService> _logger;

    public FirebaseNotificationService(
        IUnitOfWork unitOfWork,
        ILogger<FirebaseNotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SendAsync(
        int userId, string title, string body,
        Dictionary<string, string>? data = null,
        CancellationToken ct = default)
    {
        var tokens = await _unitOfWork.DeviceTokens
            .GetTokensByUserIdAsync(userId, ct);

        foreach (var token in tokens)
            await SendToTokenAsync(token, title, body, data);
    }

    private async Task SendToTokenAsync(
        string fcmToken, string title, string body,
        Dictionary<string, string>? data)
    {
        try
        {
            var message = new Message
            {
                Token = fcmToken,
                Notification = new FirebaseNotification 
                {
                    Title = title,
                    Body = body
                },
                Data = data ?? new Dictionary<string, string>(),
                Android = new AndroidConfig { Priority = Priority.High },
                Apns = new ApnsConfig { Aps = new Aps { Sound = "default" } }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "FCM send failed for token ending in ...{Suffix}",
                fcmToken.Length > 5 ? fcmToken[^5..] : fcmToken);
        }
    }

}