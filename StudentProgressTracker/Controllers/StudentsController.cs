using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentProgressTracker.DTOs;
using StudentProgressTracker.Services;
using System.ComponentModel.DataAnnotations;

namespace StudentProgressTracker.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        /// <summary>
        /// Get students with filtering and pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<StudentDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PagedResult<StudentDto>>> GetStudents(
            [FromQuery] string? grade,
            [FromQuery] string? subject,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? searchTerm,
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            [FromQuery] string? sortBy = "name")
        {
            try
            {
                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    return BadRequest("Start date cannot be greater than end date");
                }

                var result = await _studentService.GetStudentsAsync(
                    grade, subject, startDate, endDate, searchTerm,
                    pageNumber, pageSize, sortBy);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving students");
                return StatusCode(500, "An error occurred while retrieving students");
            }
        }

        /// <summary>
        /// Get detailed student information by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StudentDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid student ID");
                }

                var student = await _studentService.GetStudentByIdAsync(id);
                if (student == null)
                {
                    return NotFound($"Student with ID {id} not found");
                }

                return Ok(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student with ID {StudentId}", id);
                return StatusCode(500, "An error occurred while retrieving the student");
            }
        }

        /// <summary>
        /// Get student progress metrics
        /// </summary>
        [HttpGet("{id}/progress")]
        [ProducesResponseType(typeof(List<StudentProgressDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<StudentProgressDto>>> GetStudentProgress(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid student ID");
                }

                var progress = await _studentService.GetStudentProgressAsync(id);
                return Ok(progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving progress for student {StudentId}", id);
                return StatusCode(500, "An error occurred while retrieving student progress");
            }
        }

        /// <summary>
        /// Update student progress data
        /// </summary>
        [HttpPost("{id}/progress")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdateStudentProgress(int id,
            [FromBody] CreateStudentProgressDto progressDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid student ID");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _studentService.UpdateStudentProgressAsync(id, progressDto);
                if (!success)
                {
                    return NotFound($"Student with ID {id} not found");
                }

                return Ok(new { message = "Progress updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating progress for student {StudentId}", id);
                return StatusCode(500, "An error occurred while updating student progress");
            }
        }
    }
}
