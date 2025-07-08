namespace StudentProgressTracker.DTOs
{
    public class ProgressTrendDto
    {
        public DateTime Date { get; set; }
        public decimal AverageCompletionPercentage { get; set; }
        public decimal AveragePerformanceScore { get; set; }
        public int ActiveStudents { get; set; }
        public Dictionary<string, decimal> SubjectPerformance { get; set; } = new();
    }
}
