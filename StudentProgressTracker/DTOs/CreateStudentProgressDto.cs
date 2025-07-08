using System.ComponentModel.DataAnnotations;

namespace StudentProgressTracker.DTOs
{
    public class CreateStudentProgressDto
    {
        [Required]
        [StringLength(50)]
        public string Subject { get; set; } = string.Empty;

        [Range(0, 100)]
        public decimal CompletionPercentage { get; set; }

        [Range(0, 100)]
        public decimal PerformanceScore { get; set; }

        [Range(0, int.MaxValue)]
        public int TimeSpentMinutes { get; set; }

        [Range(0, 100)]
        public decimal AssignmentCompletionRate { get; set; }

        [Range(0, 100)]
        public decimal AssessmentScore { get; set; }
    }
}
