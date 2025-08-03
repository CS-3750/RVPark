using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RVPark.Application;
using RVPark.Core.Interfaces;
using RVPark.Core.Models;
using System.Security.Claims;

namespace RVPark.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTrackingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public TimeTrackingController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpPost("entry")]
        public async Task<IActionResult> CreateTimeEntry([FromBody] CreateTimeEntryRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                // Validate user has access to the task/project
                var task = await _context.ProjectTasks
                    .Include(t => t.Project)
                    .FirstOrDefaultAsync(t => t.Id == request.ProjectTaskId);

                if (task == null) return NotFound("Task not found");

                var hasAccess = await _context.ProjectUsers
                    .AnyAsync(pu => pu.ApplicationUserId == userId && pu.ProjectId == task.ProjectId);

                if (!hasAccess) return Forbid("You don't have access to this project");

                // Check for existing time entry on the same date for the same task
                var existingEntry = await _context.TimeEntries
                    .FirstOrDefaultAsync(te => te.ApplicationUserId == userId && 
                                             te.ProjectTaskId == request.ProjectTaskId && 
                                             te.Date.Date == request.Date.Date);

                if (existingEntry != null)
                {
                    // Update existing entry by adding hours
                    existingEntry.Hours += request.Hours;
                    existingEntry.Description = request.Description ?? existingEntry.Description;
                    existingEntry.UpdatedAt = DateTime.UtcNow;
                    _context.TimeEntries.Update(existingEntry);
                }
                else
                {
                    // Create new entry
                    var timeEntry = new TimeEntry
                    {
                        ApplicationUserId = userId,
                        ProjectTaskId = request.ProjectTaskId,
                        Hours = request.Hours,
                        Date = request.Date.Date,
                        Description = request.Description,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.TimeEntries.Add(timeEntry);
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Time entry recorded successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating time entry: {ex.Message}");
            }
        }

        [HttpGet("task/{taskId}/entries")]
        public async Task<IActionResult> GetTaskTimeEntries(int taskId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                // Validate user has access to the task
                var task = await _context.ProjectTasks
                    .Include(t => t.Project)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null) return NotFound("Task not found");

                var hasAccess = await _context.ProjectUsers
                    .AnyAsync(pu => pu.ApplicationUserId == userId && pu.ProjectId == task.ProjectId);

                if (!hasAccess) return Forbid("You don't have access to this project");

                var baseQuery = _context.TimeEntries
                    .Where(te => te.ProjectTaskId == taskId);

                if (startDate.HasValue)
                    baseQuery = baseQuery.Where(te => te.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    baseQuery = baseQuery.Where(te => te.Date <= endDate.Value.Date);

                var entries = await baseQuery
                    .Include(te => te.ApplicationUser)
                    .OrderByDescending(te => te.Date)
                    .ThenByDescending(te => te.CreatedAt)
                    .Select(te => new
                    {
                        te.Id,
                        te.Hours,
                        te.Date,
                        te.Description,
                        te.CreatedAt,
                        te.UpdatedAt,
                        UserName = te.ApplicationUser.UserName ?? "Unknown",
                        FullName = $"{te.ApplicationUser.FirstName} {te.ApplicationUser.LastName}".Trim(),
                        FormattedHours = te.Hours == 1 ? "1 hour" : $"{te.Hours:0.##} hours"
                    })
                    .ToListAsync();

                return Ok(entries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving time entries: {ex.Message}");
            }
        }

        [HttpGet("user/entries")]
        public async Task<IActionResult> GetUserTimeEntries(DateTime? startDate = null, DateTime? endDate = null, int? projectId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var baseQuery = _context.TimeEntries
                    .Where(te => te.ApplicationUserId == userId);

                if (startDate.HasValue)
                    baseQuery = baseQuery.Where(te => te.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    baseQuery = baseQuery.Where(te => te.Date <= endDate.Value.Date);

                if (projectId.HasValue)
                    baseQuery = baseQuery.Where(te => te.ProjectTask.ProjectId == projectId.Value);

                var entries = await baseQuery
                    .Include(te => te.ProjectTask)
                    .ThenInclude(pt => pt.Project)
                    .OrderByDescending(te => te.Date)
                    .ThenByDescending(te => te.CreatedAt)
                    .Select(te => new
                    {
                        te.Id,
                        te.Hours,
                        te.Date,
                        te.Description,
                        te.CreatedAt,
                        te.UpdatedAt,
                        TaskTitle = te.ProjectTask.Title,
                        ProjectTitle = te.ProjectTask.Project.Title,
                        ProjectId = te.ProjectTask.Project.Id,
                        TaskId = te.ProjectTask.Id,
                        FormattedHours = te.Hours == 1 ? "1 hour" : $"{te.Hours:0.##} hours"
                    })
                    .ToListAsync();

                return Ok(entries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user time entries: {ex.Message}");
            }
        }

        [HttpGet("project/{projectId}/summary")]
        public async Task<IActionResult> GetProjectTimeSummary(int projectId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                // Validate user has access to the project
                var hasAccess = await _context.ProjectUsers
                    .AnyAsync(pu => pu.ApplicationUserId == userId && pu.ProjectId == projectId);

                if (!hasAccess) return Forbid("You don't have access to this project");

                var baseQuery = _context.TimeEntries
                    .Where(te => te.ProjectTask.ProjectId == projectId);

                if (startDate.HasValue)
                    baseQuery = baseQuery.Where(te => te.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    baseQuery = baseQuery.Where(te => te.Date <= endDate.Value.Date);

                var entries = await baseQuery
                    .Include(te => te.ProjectTask)
                    .Include(te => te.ApplicationUser)
                    .ToListAsync();

                var summary = new
                {
                    TotalHours = entries.Sum(te => te.Hours),
                    TotalEntries = entries.Count,
                    UserSummary = entries
                        .GroupBy(te => new { te.ApplicationUserId, te.ApplicationUser.UserName, te.ApplicationUser.FirstName, te.ApplicationUser.LastName })
                        .Select(g => new
                        {
                            UserId = g.Key.ApplicationUserId,
                            UserName = g.Key.UserName ?? "Unknown",
                            FullName = $"{g.Key.FirstName} {g.Key.LastName}".Trim(),
                            TotalHours = g.Sum(te => te.Hours),
                            EntryCount = g.Count()
                        }),
                    TaskSummary = entries
                        .GroupBy(te => new { te.ProjectTaskId, te.ProjectTask.Title })
                        .Select(g => new
                        {
                            TaskId = g.Key.ProjectTaskId,
                            TaskTitle = g.Key.Title,
                            TotalHours = g.Sum(te => te.Hours),
                            EntryCount = g.Count(),
                            UserCount = g.Select(te => te.ApplicationUserId).Distinct().Count()
                        })
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving project time summary: {ex.Message}");
            }
        }

        [HttpPut("entry/{entryId}")]
        public async Task<IActionResult> UpdateTimeEntry(int entryId, [FromBody] UpdateTimeEntryRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var timeEntry = await _context.TimeEntries
                    .Include(te => te.ProjectTask)
                    .FirstOrDefaultAsync(te => te.Id == entryId && te.ApplicationUserId == userId);

                if (timeEntry == null) return NotFound("Time entry not found");

                timeEntry.Hours = request.Hours;
                timeEntry.Description = request.Description;
                timeEntry.UpdatedAt = DateTime.UtcNow;

                _context.TimeEntries.Update(timeEntry);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Time entry updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating time entry: {ex.Message}");
            }
        }

        [HttpDelete("entry/{entryId}")]
        public async Task<IActionResult> DeleteTimeEntry(int entryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var timeEntry = await _context.TimeEntries
                    .FirstOrDefaultAsync(te => te.Id == entryId && te.ApplicationUserId == userId);

                if (timeEntry == null) return NotFound("Time entry not found");

                _context.TimeEntries.Remove(timeEntry);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Time entry deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting time entry: {ex.Message}");
            }
        }
    }

    public class CreateTimeEntryRequest
    {
        public int ProjectTaskId { get; set; }
        public decimal Hours { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateTimeEntryRequest
    {
        public decimal Hours { get; set; }
        public string? Description { get; set; }
    }
}