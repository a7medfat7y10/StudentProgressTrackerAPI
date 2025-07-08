using StudentProgressTracker.Controllers;

namespace StudentProgressTracker.DTOs
{
    public class DashboardAnalytics
    {
        public int TotalStudents { get; set; }
        public int ActiveStudents { get; set; }
        public double AveragePerformance { get; set; }
        public double AverageCompletion { get; set; }
        public List<TopPerformer> TopPerformers { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }
}
