namespace Infrastructure.Settings
{
    public class PaymobSettings
    {
        public string SecretKey { get; set; } = default!;
        public int IntegrationId { get; set; }
        public string HmacSecret { get; set; } = default!;
        public string BaseUrl { get; set; } = default!;
        public string NotificationUrl { get; set; } = default!;
    }
}
