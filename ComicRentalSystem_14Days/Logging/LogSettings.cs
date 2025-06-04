using System.Text.Json.Serialization;

namespace ComicRentalSystem_14Days.Logging
{
    public class LogSettings
    {
        public int RetentionDays { get; set; } = 90;
    }
}
