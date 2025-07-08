namespace StudentProgressTracker.DTOs
{
    public class StudentProgressDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public decimal CompletionPercentage { get; set; }
        public decimal PerformanceScore { get; set; }
        public int TimeSpentMinutes { get; set; }
        public decimal AssignmentCompletionRate { get; set; }
        public decimal AssessmentScore { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
