using StudentProgressTracker.DTOs;

namespace StudentProgressTracker.Services
{
    public interface IStudentService
    {
        Task<PagedResult<StudentDto>> GetStudentsAsync(string? grade, string? subject,
            DateTime? startDate, DateTime? endDate, string? searchTerm,
            int pageNumber, int pageSize, string? sortBy);
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task<List<StudentProgressDto>> GetStudentProgressAsync(int studentId);
        Task<bool> UpdateStudentProgressAsync(int studentId, CreateStudentProgressDto progressDto);
    }
}
