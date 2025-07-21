using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RVPark.Application;
using RVPark.Core.Models;
using RVPark.Core.Utilities;
using System.Security.Claims;

namespace RVPark.Web.Pages.Shared.Projects
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _UnitOfWork;
        public List<Project> Projects { get; set; }
        public List<SelectListItem> StatusOptions { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedStatus { get; set; } = "All";

        public IndexModel(UnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }

        public IActionResult OnGet()
        {
            var items = Enum.GetValues(typeof(ProjectStatus))
                .Cast<ProjectStatus>()
                .Select(s => new SelectListItem {
                    Value = s.ToString(),
                    Text = s.GetDisplayName(),
                })
                .ToList();
            StatusOptions = new List<SelectListItem> { new SelectListItem { Value = "All", Text = "All" } };
            StatusOptions.AddRange(items);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Projects = _UnitOfWork.Project.GetAll(p => p.ProjectUsers.Any(pu => pu.ApplicationUserId == userId)).ToList();
            return Page();
        }
    }
}
