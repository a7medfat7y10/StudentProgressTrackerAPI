namespace StudentProgressTracker.DTOs
{
    public class StudentExportDto
    {
        public string FullName { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public decimal CompletionPercentage { get; set; }
        public decimal PerformanceScore { get; set; }
        public int TimeSpentMinutes { get; set; }
        public decimal AssignmentCompletionRate { get; set; }
        public decimal AssessmentScore { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
