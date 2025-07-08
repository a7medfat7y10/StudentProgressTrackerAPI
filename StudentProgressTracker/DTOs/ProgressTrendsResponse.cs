using StudentProgressTracker.Controllers;

namespace StudentProgressTracker.DTOs
{
    public class ProgressTrendsResponse
    {
        public List<TrendDataPoint> TrendData { get; set; } = new();
        public List<SubjectTrend> SubjectTrends { get; set; } = new();
        public DateRange DateRange { get; set; } = new();
        public string Interval { get; set; } = string.Empty;
    }
}
