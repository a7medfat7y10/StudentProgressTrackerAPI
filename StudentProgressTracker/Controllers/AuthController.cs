using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentProgressTracker.DTOs;
using StudentProgressTracker.Services;

namespace StudentProgressTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.Identity?.Name;
            if (username == null) return Unauthorized();

            var profile = await _authService.GetProfileAsync(username);
            return profile != null ? Ok(profile) : NotFound();
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet("/api/users/{id}/students")]
        public async Task<IActionResult> GetStudentsForTeacher(string id)
        {
            var students = await _authService.GetStudentsForTeacherAsync(id);
            return Ok(students);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-teacher")]
        public async Task<IActionResult> AssignTeacherRole([FromBody] RoleAssignmentDto dto)
        {
            try
            {
                var result = await _authService.AssignRoleAsync(dto.Email, "Teacher");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning teacher role");
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-admin")]
        public async Task<IActionResult> AssignAdminRole([FromBody] RoleAssignmentDto dto)
        {
            try
            {
                var result = await _authService.AssignRoleAsync(dto.Email, "Admin");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning admin role");
                return BadRequest(ex.Message);
            }
        }
    }
}
