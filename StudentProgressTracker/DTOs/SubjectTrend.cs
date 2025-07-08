namespace StudentProgressTracker.DTOs
{
    public class SubjectTrend
    {
        public string Subject { get; set; } = string.Empty;
        public List<TrendDataPoint> TrendPoints { get; set; } = new();
    }
}
