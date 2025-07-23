using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RVPark.Application;
using RVPark.Core.Models;

namespace RVPark.Web.Pages.Shared.Projects.ProjectTasks
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _UnitOfWork;

        [BindProperty(SupportsGet = true)]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public List<ProjectTask> Tasks { get; set; }

        public IndexModel(UnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }

        public void OnGet()
        {
            Project = _UnitOfWork.Project.GetById(ProjectId);
            Tasks = _UnitOfWork.ProjectTask
                .GetAll(pt => pt.ProjectId == ProjectId)
                .OrderBy(ProjectId => ProjectId.StartDate)
                .ToList();
        }
    }
}
