using Application.Interfaces;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;


namespace Warsha.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
        => _settings = settings.Value;


    public Task SendOtpAsync(
        string email, string name, string otpCode, CancellationToken ct = default)
    {
        var content = $@"
            <p style='font-size:16px;color:#333'>Hello <strong>{name}</strong>,</p>
            <p style='color:#666'>Use the following code to confirm your account:</p>
            {OtpBlock(otpCode)}
            <p style='color:#999;font-size:13px'>The code is valid for 10 minutes only.</p>";

        return SendAsync(email, "Email Verification — Warsha", BuildTemplate("Email Verification", content), ct);
    }

    public Task SendWorkshopApprovedAsync(
        string email, string workshopName, CancellationToken ct = default)
    {
        var content = $@"
            <p style='font-size:16px;color:#333'>Hello <strong>{workshopName}</strong>,</p>
            <p style='color:#666'>The workshop was successfully accepted on the Warsha platform.</p>
            <p style='color:#666'>You can now log in and start receiving bookings.</p>
            {SuccessBadge("Accepted ✓")}";

        return SendAsync(email, "Workshop Accepted — Warsha", BuildTemplate("Your Workshop is Ready!", content), ct);
    }

    public Task SendWorkshopRejectedAsync(
        string email, string workshopName, CancellationToken ct = default)
    {
        var content = $@"
            <p style='font-size:16px;color:#333'>Hello <strong>{workshopName}</strong>,</p>
            <p style='color:#666'>Sorry, your workshop was not accepted at this time.</p>
            <p style='color:#666'>You can contact the Warsha team for more details.</p>";

        return SendAsync(email, "Workshop Request Status — Warsha", BuildTemplate("Request Status", content), ct);
    }

    public Task SendBookingConfirmedAsync(
        string email, string clientName, string bookingCode,
        string workshopName, DateTime scheduledAt, CancellationToken ct = default)
    {
        var content = $@"
            <p style='font-size:16px;color:#333'>Hello <strong>{clientName}</strong>,</p>
            <p style='color:#666'>Your booking has been successfully confirmed.</p>
            {InfoBlock(new Dictionary<string, string>
        {
            ["Booking Code"] = bookingCode,
            ["Workshop"] = workshopName,
            ["Scheduled At"] = scheduledAt.ToString("dddd, dd MMMM yyyy — hh:mm tt")
        })}
            <p style='color:#999;font-size:13px'>You can track the status of your booking through the app.</p>";

        return SendAsync(email, $"Booking Confirmation {bookingCode} — Warsha", BuildTemplate("Booking Confirmed ✓", content), ct);
    }

    public Task SendQuoteReceivedAsync(
        string email, string clientName, string bookingCode,
        decimal extraAmount, CancellationToken ct = default)
    {
        var content = $@"
            <p style='font-size:16px;color:#333'>Hello <strong>{clientName}</strong>,</p>
            <p style='color:#666'>The workshop has sent an additional quote for booking <strong>{bookingCode}</strong>.</p>
            {InfoBlock(new Dictionary<string, string>
        {
            ["Additional Cost"] = $"{extraAmount:N0} EGP"
        })}
            <p style='color:#666'>Open the app to approve or reject.</p>";

        return SendAsync(email, $"New Quote — {bookingCode}", BuildTemplate("Additional Quote", content), ct);
    }

    public Task SendDailySummaryAsync(
        string email, string workshopName, int bookingsCount,
        decimal revenue, CancellationToken ct = default)
    {
        var content = $@"
            <p style='font-size:16px;color:#333'>Hello <strong>{workshopName}</strong>,</p>
            <p style='color:#666'>Your daily summary on the Warsha platform:</p>
            {InfoBlock(new Dictionary<string, string>
        {
            ["Completed Bookings"] = bookingsCount.ToString(),
            ["Revenue"] = $"{revenue:N0} EGP"
        })}";

        return SendAsync(email, "Daily Summary — Warsha", BuildTemplate("Your Daily Summary", content), ct);
    }


    private async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls, ct);
        await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
        await smtp.SendAsync(message, ct);
        await smtp.DisconnectAsync(true, ct);
    }

    private static string BuildTemplate(string title, string content) => $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
            <div style='background:linear-gradient(135deg,#F97316,#EA580C);
                        padding:28px;text-align:center;border-radius:10px 10px 0 0;'>
                <h1 style='color:white;margin:0;font-size:22px;'>Warsha</h1>
                <p style='color:rgba(255,255,255,0.85);margin:6px 0 0;font-size:14px;'>{title}</p>
            </div>
            <div style='background:#f9f9f9;padding:28px;border-radius:0 0 10px 10px;'>
                {content}
                <hr style='border:none;border-top:1px solid #ddd;margin:24px 0;'>
                <p style='font-size:11px;color:#aaa;text-align:center;'>
                    © {DateTime.UtcNow.Year} Warsha. All rights reserved.
                </p>
            </div>
        </div>";

    private static string OtpBlock(string otp) => $@"
        <div style='background:white;padding:20px;text-align:center;
                    margin:20px 0;border-radius:8px;box-shadow:0 2px 4px rgba(0,0,0,0.1);'>
            <p style='font-size:13px;color:#666;margin-bottom:8px;'>Otp code:</p>
            <h2 style='color:#F97316;font-size:36px;letter-spacing:8px;margin:0;'>{otp}</h2>
        </div>";

    private static string SuccessBadge(string text) => $@"
        <div style='background:#d1fae5;color:#065f46;padding:12px 20px;
                    border-radius:8px;text-align:center;font-weight:bold;margin:16px 0;'>
            {text}
        </div>";

    private static string InfoBlock(Dictionary<string, string> items)
    {
        var rows = items.Select(kvp => $@"
            <tr>
                <td style='padding:8px 12px;color:#666;font-size:13px;
                           border-bottom:1px solid #eee;white-space:nowrap;'>{kvp.Key}</td>
                <td style='padding:8px 12px;color:#333;font-size:13px;
                           border-bottom:1px solid #eee;font-weight:bold;'>{kvp.Value}</td>
            </tr>");

        return $@"
            <table style='width:100%;border-collapse:collapse;
                          background:white;border-radius:8px;
                          margin:16px 0;overflow:hidden;'>
                {string.Join("", rows)}
            </table>";
    }
}