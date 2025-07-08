namespace StudentProgressTracker.DTOs
{
    public class TrendDataPoint
    {
        public DateTime Date { get; set; }
        public double AverageCompletion { get; set; }
        public double AveragePerformance { get; set; }
        public int StudentCount { get; set; }
    }
}
