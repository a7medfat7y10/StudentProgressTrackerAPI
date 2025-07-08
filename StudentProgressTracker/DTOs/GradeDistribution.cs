namespace StudentProgressTracker.DTOs
{
    public class GradeDistribution
    {
        public string Grade { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public double AverageCompletion { get; set; }
        public double AveragePerformance { get; set; }
    }
}
