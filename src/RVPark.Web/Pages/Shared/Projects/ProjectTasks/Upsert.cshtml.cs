using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RVPark.Application;
using RVPark.Core.Models;
using System;
using System.Threading.Tasks;

namespace RVPark.Pages.Projects.Tasks
{
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _UnitOfWork;

        public UpsertModel(UnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }

        [BindProperty(SupportsGet = true)]
        public int ProjectId { get; set; }

        public Project? Project { get; set; }

        // Optional id: null = create, has value = edit
        [BindProperty(SupportsGet = true)]
        public int? ProjectTaskId { get; set; }

        [BindProperty]
        public ProjectTask Task { get; set; }

        public IActionResult OnGet()
        {
            Project = _UnitOfWork.Project.GetById(ProjectId);
            if (ProjectTaskId.HasValue && ProjectTaskId.Value > 0)
            {
                // Editing existing
                Task = _UnitOfWork.ProjectTask.GetById(ProjectTaskId.Value);
                if (Task == null)
                    return NotFound();
            }
            else
            {
                // Creating new
                Task = new ProjectTask
                {
                    ProjectId = ProjectId,
                    Title = "New Task",
                    Description = "Task description goes here",
                };
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Task.Id > 0)
            {
                // Update
                _UnitOfWork.ProjectTask.Update(Task);
            }
            else
            {
                // Insert
                _UnitOfWork.ProjectTask.Add(Task);
            }
            return RedirectToPage("/Shared/Projects/Upsert", new { id = ProjectId });
        }
    }
}
