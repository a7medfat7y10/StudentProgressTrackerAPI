namespace StudentProgressTracker.DTOs
{
    public class StudentProgressExport
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public double CompletionPercentage { get; set; }
        public double PerformanceScore { get; set; }
        public int TimeSpentMinutes { get; set; }
        public double AssignmentCompletionRate { get; set; }
        public double AssessmentScore { get; set; }
        public DateTime RecordedDate { get; set; }
        public DateTime LastActivityDate { get; set; }
    }
}
