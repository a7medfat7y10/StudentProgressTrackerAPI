using Microsoft.EntityFrameworkCore;
using StudentProgressTracker.Data;
using StudentProgressTracker.DTOs;
using StudentProgressTracker.Models;

namespace StudentProgressTracker.Services
{

    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(ApplicationDbContext context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<StudentDto>> GetStudentsAsync(string? grade, string? subject,
            DateTime? startDate, DateTime? endDate, string? searchTerm,
            int pageNumber, int pageSize, string? sortBy)
        {
            try
            {
                var query = _context.Students
                    .Include(s => s.Progress)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(grade) && int.TryParse(grade, out var gradeInt))
                {
                    query = query.Where(s => s.Grade == gradeInt);
                }

                if (!string.IsNullOrEmpty(subject))
                {
                    query = query.Where(s => s.Progress.Any(p => p.Subject.ToLower() == subject.ToLower()));
                }

                if (startDate.HasValue)
                {
                    query = query.Where(s => s.Progress.Any(p => p.LastActivity >= startDate.Value));
                }

                if (endDate.HasValue)
                {
                    query = query.Where(s => s.Progress.Any(p => p.LastActivity <= endDate.Value));
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var searchLower = searchTerm.ToLower();
                    query = query.Where(s =>
                        s.FullName.ToLower().Contains(searchLower) ||
                        s.Email.ToLower().Contains(searchLower));
                }

                // Apply sorting
                query = sortBy?.ToLower() switch
                {
                    "name" => query.OrderBy(s => s.FullName),
                    "grade" => query.OrderBy(s => s.Grade),
                    "progress" => query.OrderByDescending(s => s.Progress.Average(p => p.CompletionPercentage)),
                    "lastactivity" => query.OrderByDescending(s => s.Progress.Max(p => p.LastActivity)),
                    _ => query.OrderBy(s => s.FullName)
                };

                var totalCount = await query.CountAsync();

                var students = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var studentDtos = students.Select(MapToStudentDto).ToList();

                return new PagedResult<StudentDto>
                {
                    Items = studentDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving students");
                throw;
            }
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.Progress)
                    .FirstOrDefaultAsync(s => s.Id == id);

                return student == null ? null : MapToStudentDto(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student with ID {StudentId}", id);
                throw;
            }
        }

        public async Task<List<StudentProgressDto>> GetStudentProgressAsync(int studentId)
        {
            try
            {
                var progress = await _context.StudentProgress
                    .Where(p => p.StudentId == studentId)
                    .OrderBy(p => p.Subject)
                    .ToListAsync();

                return progress.Select(MapToStudentProgressDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving progress for student {StudentId}", studentId);
                throw;
            }
        }

        public async Task<bool> UpdateStudentProgressAsync(int studentId, CreateStudentProgressDto progressDto)
        {
            try
            {
                var student = await _context.Students.FindAsync(studentId);
                if (student == null)
                {
                    return false;
                }

                var existingProgress = await _context.StudentProgress
                    .FirstOrDefaultAsync(p => p.StudentId == studentId && p.Subject == progressDto.Subject);

                if (existingProgress != null)
                {
                    // Update existing progress
                    existingProgress.CompletionPercentage = progressDto.CompletionPercentage;
                    existingProgress.PerformanceScore = progressDto.PerformanceScore;
                    existingProgress.TimeSpentMinutes = progressDto.TimeSpentMinutes;
                    existingProgress.AssignmentCompletionRate = progressDto.AssignmentCompletionRate;
                    existingProgress.AssessmentScore = progressDto.AssessmentScore;
                    existingProgress.LastActivity = DateTime.UtcNow;
                    existingProgress.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new progress record
                    var newProgress = new StudentProgress
                    {
                        StudentId = studentId,
                        Subject = progressDto.Subject,
                        CompletionPercentage = progressDto.CompletionPercentage,
                        PerformanceScore = progressDto.PerformanceScore,
                        TimeSpentMinutes = progressDto.TimeSpentMinutes,
                        AssignmentCompletionRate = progressDto.AssignmentCompletionRate,
                        AssessmentScore = progressDto.AssessmentScore,
                        LastActivity = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.StudentProgress.Add(newProgress);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating progress for student {StudentId}", studentId);
                throw;
            }
        }

        private static StudentDto MapToStudentDto(Student student)
        {
            var progressList = student.Progress.Select(MapToStudentProgressDto).ToList();

            return new StudentDto
            {
                Id = student.Id,
                FirstName = student.FullName,
                Grade = student.Grade,
                Email = student.Email,
                CreatedAt = student.CreatedAt,
                UpdatedAt = student.UpdatedAt,
                Progress = progressList,
                OverallCompletionPercentage = progressList.Any() ? progressList.Average(p => p.CompletionPercentage) : 0,
                OverallPerformanceScore = progressList.Any() ? progressList.Average(p => p.PerformanceScore) : 0,
                LastActivity = progressList.Any() ? progressList.Max(p => p.LastActivity) : student.CreatedAt
            };
        }

        private static StudentProgressDto MapToStudentProgressDto(StudentProgress progress)
        {
            return new StudentProgressDto
            {
                Id = progress.Id,
                StudentId = progress.StudentId,
                Subject = progress.Subject,
                CompletionPercentage = progress.CompletionPercentage,
                PerformanceScore = progress.PerformanceScore,
                TimeSpentMinutes = progress.TimeSpentMinutes,
                AssignmentCompletionRate = progress.AssignmentCompletionRate,
                AssessmentScore = progress.AssessmentScore,
                LastActivity = progress.LastActivity,
                CreatedAt = progress.CreatedAt,
                UpdatedAt = progress.UpdatedAt
            };
        }
    }
}
