using System.ComponentModel.DataAnnotations;

namespace StudentProgressTracker.DTOs
{
    public class AnalyticsFilterRequest
    {
        public string? Grade { get; set; }
        public string? Subject { get; set; }
        public int? TeacherId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
    public class TrendsFilterRequest : AnalyticsFilterRequest
    {
        public int? StudentId { get; set; }

        [RegularExpression("^(daily|weekly|monthly)$", ErrorMessage = "Interval must be 'daily', 'weekly', or 'monthly'")]
        public string Interval { get; set; } = "weekly";
    }

    public class ExportFilterRequest : AnalyticsFilterRequest
    {
        public string Format { get; set; } = "csv";
    }
}
