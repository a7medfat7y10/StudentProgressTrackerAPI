using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentProgressTracker.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; } = string.Empty;

        [Range(1, 12)]
        public int Grade { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<StudentProgress> Progress { get; set; } = new List<StudentProgress>();
        [Required]
        public string AssignedTeacherId { get; set; } = null!;

        [ForeignKey(nameof(AssignedTeacherId))]
        public virtual IdentityUser AssignedTeacher { get; set; } = null!;
    }
}
