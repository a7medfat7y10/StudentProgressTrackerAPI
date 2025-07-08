using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentProgressTracker.Data;
using StudentProgressTracker.DTOs;
using StudentProgressTracker.Services;
using System.Text;

namespace StudentProgressTracker.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        /// <summary>
        /// Get class-level statistics
        /// </summary>
        [HttpGet("class-summary")]
        [ProducesResponseType(typeof(ClassSummaryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClassSummaryDto>> GetClassSummary(
            [FromQuery] int? grade,
            [FromQuery] string? subject)
        {
            try
            {
                if (grade.HasValue && (grade < 1 || grade > 12))
                {
                    return BadRequest("Grade must be between 1 and 12");
                }

                var summary = await _analyticsService.GetClassSummaryAsync(grade, subject);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating class summary");
                return StatusCode(500, "An error occurred while generating class summary");
            }
        }

        /// <summary>
        /// Get historical progress data
        /// </summary>
        [HttpGet("progress-trends")]
        [ProducesResponseType(typeof(List<ProgressTrendDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<ProgressTrendDto>>> GetProgressTrends(
            [FromQuery] int? grade,
            [FromQuery] string? subject,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                if (grade.HasValue && (grade < 1 || grade > 12))
                {
                    return BadRequest("Grade must be between 1 and 12");
                }

                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    return BadRequest("Start date cannot be greater than end date");
                }

                var trends = await _analyticsService.GetProgressTrendsAsync(grade, subject, startDate, endDate);
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating progress trends");
                return StatusCode(500, "An error occurred while generating progress trends");
            }
        }
 
        [HttpGet("student-export")]
        public async Task<IActionResult> ExportStudentData([FromQuery] int? grade = null,
            [FromQuery] string? subject = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            try
            {
                var data = await _analyticsService.GetStudentExportDataAsync(grade, subject, dateFrom, dateTo);
                var csv = await _analyticsService.GenerateCsvAsync(data);
                var fileName = $"student_progress_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

                return File(Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting student data");
                return StatusCode(500, new { error = "Internal server error while exporting student data" });
            }
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardAnalytics>> GetDashboardAnalytics()
        {
            try
            {
                var result = await _analyticsService.GetDashboardAnalyticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard analytics");
                return StatusCode(500, new { error = "Internal server error while retrieving dashboard analytics" });
            }
        }
    }
}
