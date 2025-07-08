namespace StudentProgressTracker.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public int Grade { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<StudentProgressDto> Progress { get; set; } = new();
        public decimal OverallCompletionPercentage { get; set; }
        public decimal OverallPerformanceScore { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
