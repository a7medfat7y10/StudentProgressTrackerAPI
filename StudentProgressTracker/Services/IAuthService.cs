using StudentProgressTracker.Models;

namespace StudentProgressTracker.Services
{
    public interface IAuthService
    {
        Task<object?> GetProfileAsync(string username);
        Task<List<Student>> GetStudentsForTeacherAsync(string teacherId);
        Task<string> AssignRoleAsync(string email, string role);
    }
}
