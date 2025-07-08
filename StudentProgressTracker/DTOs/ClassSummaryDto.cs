namespace StudentProgressTracker.DTOs
{
    public class ClassSummaryDto
    {
        public int TotalStudents { get; set; }
        public int Grade { get; set; }
        public decimal AverageCompletionPercentage { get; set; }
        public decimal AveragePerformanceScore { get; set; }
        public int TotalTimeSpentMinutes { get; set; }
        public Dictionary<string, SubjectSummaryDto> SubjectSummaries { get; set; } = new();
        public Dictionary<string, int> PerformanceLevels { get; set; } = new(); // Advanced, OnTrack, Struggling
    }
}
