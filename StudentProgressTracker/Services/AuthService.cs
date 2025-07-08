using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentProgressTracker.Data;
using StudentProgressTracker.Models;

namespace StudentProgressTracker.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AuthService(UserManager<IdentityUser> userManager,
                           RoleManager<IdentityRole> roleManager,
                           ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<object?> GetProfileAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new
            {
                user.UserName,
                user.Email,
                Roles = roles
            };
        }

        public async Task<List<Student>> GetStudentsForTeacherAsync(string teacherId)
        {
            return await _context.Students
                .Where(s => s.AssignedTeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<string> AssignRoleAsync(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found");

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            var result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
                throw new Exception($"Failed to assign role {role} to {email}");

            return $"Assigned '{role}' role to {email}";
        }
    }
}
