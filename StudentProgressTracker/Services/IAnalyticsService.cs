using StudentProgressTracker.DTOs;

namespace StudentProgressTracker.Services
{
    public interface IAnalyticsService
    {
        Task<ClassSummaryDto> GetClassSummaryAsync(int? grade = null, string? subject = null);
        Task<List<ProgressTrendDto>> GetProgressTrendsAsync(int? grade = null, string? subject = null,
            DateTime? startDate = null, DateTime? endDate = null);
        Task<List<StudentExportDto>> GetStudentExportDataAsync(int? grade = null, string? subject = null,
            DateTime? startDate = null, DateTime? endDate = null);
        Task<DashboardAnalytics> GetDashboardAnalyticsAsync();
        Task<string> GenerateCsvAsync(List<StudentExportDto> data);
    }
}
