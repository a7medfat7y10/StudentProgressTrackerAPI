using System.ComponentModel.DataAnnotations;

namespace StudentProgressTracker.Models
{
    public class StudentProgress
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

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

        public DateTime LastActivity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual Student Student { get; set; } = null!;
    }
}
