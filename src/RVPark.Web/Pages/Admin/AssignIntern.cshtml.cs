using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RVPark.Core.Interfaces;
using RVPark.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RVPark.Application.Pages.Admin
{
    public class AssignInternModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssignInternModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<ProjectViewModel> Projects { get; set; }
        public List<InternViewModel> Interns { get; set; }

        [BindProperty]
        public string SelectedProjectId { get; set; }

        [BindProperty]
        public string SelectedLeadInternId { get; set; } // null or empty means remove lead

        public string CurrentLeadInternId { get; set; }

        public async Task OnGetAsync(string? projectId = null)
        {
            var projects = await _unitOfWork.Project.GetAllAsync();
            Projects = projects.Select(p => new ProjectViewModel
            {
                Id = p.Id.ToString(), // Ensure as string for selection
                Name = p.Name
            }).ToList();

            var interns = await _unitOfWork.User.GetAllAsync();
            Interns = interns
                .Where(u => u is ApplicationUser appUser && appUser.IsActive) // Filter only active users
                .Select(u => new InternViewModel
                {
                    Id = u.Id,
                    Name = $"{u.FirstName} {u.LastName}".Trim()
                }).ToList();

            SelectedProjectId = projectId ?? Projects.FirstOrDefault()?.Id ?? string.Empty;

            if (!string.IsNullOrEmpty(SelectedProjectId))
            {
                // If your Project.Id is int or Guid, parse accordingly:
                var project = await _unitOfWork.Project.GetAsync(p => p.Id.ToString() == SelectedProjectId);
                CurrentLeadInternId = project?.LeadInternId ?? string.Empty;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(SelectedProjectId))
            {
                ModelState.AddModelError("", "Please select a project.");
                await OnGetAsync();
                return Page();
            }

            // If your Project.Id is int or Guid, parse accordingly:
            var project = await _unitOfWork.Project.GetAsync(p => p.Id.ToString() == SelectedProjectId);
            if (project == null)
            {
                ModelState.AddModelError("", "Project not found.");
                await OnGetAsync();
                return Page();
            }

            // Assign or remove lead intern
            project.LeadInternId = string.IsNullOrEmpty(SelectedLeadInternId) ? null : SelectedLeadInternId;
            _unitOfWork.Project.Update(project);
            // await _unitOfWork.SaveAsync(); // Uncomment if you use SaveAsync

            return RedirectToPage(new { projectId = SelectedProjectId });
        }

        public class InternViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class ProjectViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}