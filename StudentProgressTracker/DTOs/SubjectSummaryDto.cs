namespace StudentProgressTracker.DTOs
{
    public class SubjectSummaryDto
    {
        public string Subject { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public decimal AverageCompletionPercentage { get; set; }
        public decimal AveragePerformanceScore { get; set; }
        public int TotalTimeSpentMinutes { get; set; }
        public decimal AverageAssignmentCompletionRate { get; set; }
        public decimal AverageAssessmentScore { get; set; }
    }
}
