using Microsoft.EntityFrameworkCore;
using StudentProgressTracker.Data;
using StudentProgressTracker.DTOs;
using System.Text;

namespace StudentProgressTracker.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(ApplicationDbContext context, ILogger<AnalyticsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ClassSummaryDto> GetClassSummaryAsync(int? grade = null, string? subject = null)
        {
            try
            {
                var studentsQuery = _context.Students
                    .Include(s => s.Progress)
                    .AsQueryable();

                if (grade.HasValue)
                {
                    studentsQuery = studentsQuery.Where(s => s.Grade == grade.Value);
                }

                var students = await studentsQuery.ToListAsync();
                
                var progressQuery = _context.StudentProgress.AsQueryable();
                
                if (grade.HasValue)
                {
                    progressQuery = progressQuery.Where(p => p.Student.Grade == grade.Value);
                }

                if (!string.IsNullOrEmpty(subject))
                {
                    progressQuery = progressQuery.Where(p => p.Subject.ToLower() == subject.ToLower());
                }

                var progressData = await progressQuery
                    .Include(p => p.Student)
                    .ToListAsync();

                var classSummary = new ClassSummaryDto
                {
                    TotalStudents = students.Count,
                    Grade = grade ?? 0,
                    AverageCompletionPercentage = progressData.Any() ? progressData.Average(p => p.CompletionPercentage) : 0,
                    AveragePerformanceScore = progressData.Any() ? progressData.Average(p => p.PerformanceScore) : 0,
                    TotalTimeSpentMinutes = progressData.Sum(p => p.TimeSpentMinutes)
                };

                // Subject summaries
                var subjectGroups = progressData.GroupBy(p => p.Subject);
                foreach (var subjectGroup in subjectGroups)
                {
                    var subjectData = subjectGroup.ToList();
                    classSummary.SubjectSummaries[subjectGroup.Key] = new SubjectSummaryDto
                    {
                        Subject = subjectGroup.Key,
                        StudentCount = subjectData.Select(p => p.StudentId).Distinct().Count(),
                        AverageCompletionPercentage = subjectData.Average(p => p.CompletionPercentage),
                        AveragePerformanceScore = subjectData.Average(p => p.PerformanceScore),
                        TotalTimeSpentMinutes = subjectData.Sum(p => p.TimeSpentMinutes),
                        AverageAssignmentCompletionRate = subjectData.Average(p => p.AssignmentCompletionRate),
                        AverageAssessmentScore = subjectData.Average(p => p.AssessmentScore)
                    };
                }

                // Performance levels
                var performanceLevels = new Dictionary<string, int>
                {
                    ["Advanced"] = 0,
                    ["OnTrack"] = 0,
                    ["Struggling"] = 0
                };

                foreach (var student in students)
                {
                    var studentProgress = student.Progress.ToList();
                    if (studentProgress.Any())
                    {
                        var avgPerformance = studentProgress.Average(p => p.PerformanceScore);
                        if (avgPerformance >= 80)
                            performanceLevels["Advanced"]++;
                        else if (avgPerformance >= 60)
                            performanceLevels["OnTrack"]++;
                        else
                            performanceLevels["Struggling"]++;
                    }
                    else
                    {
                        performanceLevels["Struggling"]++;
                    }
                }

                classSummary.PerformanceLevels = performanceLevels;
                return classSummary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating class summary");
                throw;
            }
        }

        public async Task<List<ProgressTrendDto>> GetProgressTrendsAsync(int? grade = null, string? subject = null, 
            DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.StudentProgress
                    .Include(p => p.Student)
                    .AsQueryable();

                if (grade.HasValue)
                {
                    query = query.Where(p => p.Student.Grade == grade.Value);
                }

                if (!string.IsNullOrEmpty(subject))
                {
                    query = query.Where(p => p.Subject.ToLower() == subject.ToLower());
                }

                if (startDate.HasValue)
                {
                    query = query.Where(p => p.CreatedAt >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(p => p.CreatedAt <= endDate.Value);
                }

                var progressData = await query
                    .OrderBy(p => p.CreatedAt)
                    .ToListAsync();

                // Group by week for trend analysis
                var trends = progressData
                    .GroupBy(p => new { 
                        Year = p.CreatedAt.Year, 
                        Week = GetWeekOfYear(p.CreatedAt) 
                    })
                    .Select(g => new ProgressTrendDto
                    {
                        Date = GetFirstDayOfWeek(g.Key.Year, g.Key.Week),
                        AverageCompletionPercentage = g.Average(p => p.CompletionPercentage),
                        AveragePerformanceScore = g.Average(p => p.PerformanceScore),
                        ActiveStudents = g.Select(p => p.StudentId).Distinct().Count(),
                        SubjectPerformance = g.GroupBy(p => p.Subject)
                            .ToDictionary(sg => sg.Key, sg => sg.Average(p => p.PerformanceScore))
                    })
                    .OrderBy(t => t.Date)
                    .ToList();

                return trends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating progress trends");
                throw;
            }
        }

        public async Task<List<StudentExportDto>> GetStudentExportDataAsync(int? grade = null, string? subject = null,
    DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Students
                .Include(s => s.Progress)
                .AsQueryable();

            if (grade.HasValue)
            {
                query = query.Where(s => s.Grade == grade.Value);
            }

            var students = await query.ToListAsync();

            var filteredData = students.SelectMany(s => s.Progress
                .Where(pr =>
                    (string.IsNullOrEmpty(subject) || pr.Subject == subject) &&
                    (!startDate.HasValue || pr.CreatedAt >= startDate.Value) &&
                    (!endDate.HasValue || pr.CreatedAt <= endDate.Value))
                .Select(pr => new StudentExportDto
                {
                    FullName = s.FullName,
                    Grade = s.Grade,
                    Subject = pr.Subject,
                    CompletionPercentage = pr.CompletionPercentage,
                    PerformanceScore = pr.PerformanceScore,
                    TimeSpentMinutes = pr.TimeSpentMinutes,
                    AssignmentCompletionRate = pr.AssignmentCompletionRate,
                    AssessmentScore = pr.AssessmentScore,
                    LastActivity = pr.LastActivity
                }))
                .OrderBy(d => d.FullName)
                .ThenBy(d => d.Subject)
                .ToList();

            return filteredData;
        }

        public async Task<DashboardAnalytics> GetDashboardAnalyticsAsync()
        {
            var totalStudents = await _context.Students.CountAsync();
            var activeStudents = await _context.StudentProgress
                .Where(pr => pr.LastActivity >= DateTime.UtcNow.AddDays(-7))
                .Select(pr => pr.StudentId)
                .Distinct()
                .CountAsync();

            var recentProgress = await _context.StudentProgress
                .Where(pr => pr.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                .ToListAsync();

            var avgPerformance = recentProgress.Any() ? recentProgress.Average(pr => pr.PerformanceScore) : 0;
            var avgCompletion = recentProgress.Any() ? recentProgress.Average(pr => pr.CompletionPercentage) : 0;

            var topPerformers = await _context.Students
                .Include(s => s.Progress)
                .Select(s => new
                {
                    Student = s,
                    AvgPerformance = s.Progress.Any() ? s.Progress.Average(pr => pr.PerformanceScore) : 0
                })
                .OrderByDescending(x => x.AvgPerformance)
                .Take(5)
                .Select(x => new TopPerformer
                {
                    StudentName = x.Student.FullName,
                    Grade = x.Student.Grade.ToString(),
                    AveragePerformance = (double)Math.Round(x.AvgPerformance, 2)
                })
                .ToListAsync();

            return new DashboardAnalytics
            {
                TotalStudents = totalStudents,
                ActiveStudents = activeStudents,
                AveragePerformance = (double) Math.Round(avgPerformance, 2),
                AverageCompletion = (double) Math.Round(avgCompletion, 2),
                TopPerformers = topPerformers,
                LastUpdated = DateTime.UtcNow
            };
        }

        public Task<string> GenerateCsvAsync(List<StudentExportDto> data)
        {
            var csv = new StringBuilder();
            csv.AppendLine("StudentName,Grade,Subject,CompletionPercentage,PerformanceScore,TimeSpentMinutes,AssignmentCompletionRate,AssessmentScore,LastActivity");

            foreach (var record in data)
            {
                csv.AppendLine($"\"{record.FullName}\"," +
                              $"\"{record.Grade}\"," +
                              $"\"{record.Subject}\"," +
                              $"{record.CompletionPercentage}," +
                              $"{record.PerformanceScore}," +
                              $"{record.TimeSpentMinutes}," +
                              $"{record.AssignmentCompletionRate}," +
                              $"{record.AssessmentScore}," +
                              $"{record.LastActivity:yyyy-MM-dd HH:mm:ss}");
            }

            return Task.FromResult(csv.ToString());
        }


        private static int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            var calendarWeekRule = culture.DateTimeFormat.CalendarWeekRule;
            var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            
            return calendar.GetWeekOfYear(date, calendarWeekRule, firstDayOfWeek);
        }

        private static DateTime GetFirstDayOfWeek(int year, int weekNumber)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = (int)System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            var firstWeek = jan1.AddDays(daysOffset);
            var weekStart = firstWeek.AddDays((weekNumber - 1) * 7);
            return weekStart;
        }
    }
}
