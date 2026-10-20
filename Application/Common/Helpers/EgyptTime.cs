namespace Application.Common.Helpers
{
    public static class EgyptTime
    {
        private static readonly TimeZoneInfo Zone = Resolve();

        public static DateOnly TodayLocal() => ToLocalDate(DateTime.UtcNow);

        public static DateOnly ToLocalDate(DateTime utc)
        {
            var asUtc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(asUtc, Zone));
        }

        // UTC instant at which the given Egypt calendar day starts
        public static DateTime StartOfDayUtc(DateOnly localDate)
        {
            var unspecified = DateTime.SpecifyKind(localDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);

            // Egypt's DST starts at midnight, so 00:00 doesn't exist that day; use 01:00 instead of throwing
            if (Zone.IsInvalidTime(unspecified))
                unspecified = unspecified.AddHours(1);

            return TimeZoneInfo.ConvertTimeToUtc(unspecified, Zone);
        }

        private static TimeZoneInfo Resolve()
        {
            foreach (var id in new[] { "Africa/Cairo", "Egypt Standard Time" })
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }
            return TimeZoneInfo.Utc;
        }
    }
}