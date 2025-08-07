using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RVPark.Application;
using RVPark.Core.Models;
using System.Security.Claims;

namespace RVPark.Web.Pages.Shared.Projects
{
    [Authorize]
    public class FilesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public FilesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int projectId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // Check if user has access to this project
            var hasAccess = await _context.ProjectUsers
                .AnyAsync(pu => pu.ApplicationUserId == userId && pu.ProjectId == projectId);

            if (!hasAccess)
            {
                return Forbid("You don't have access to this project");
            }

            // Get project details
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound("Project not found");
            }

            ProjectId = projectId;
            ProjectTitle = project.Title;

            return Page();
        }
    }
}