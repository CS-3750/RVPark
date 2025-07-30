using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RVPark.Application;
using RVPark.Core.Models;
using RVPark.Core.Utilities;
using System.Security.Claims;

namespace RVPark.Web.Pages.Shared.Projects
{
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _UnitOfWork;

        [BindProperty]
        public Project Project { get; set; }
        public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

        // dropdown items for StatusEnum
        public List<SelectListItem> StatusOptions { get; set; }

        public UpsertModel(UnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }

        private void PopulateStatusOptions()
        {
            // build StatusOptions from enum
            StatusOptions = Enum.GetValues(typeof(ProjectStatus))
                .Cast<ProjectStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.GetDisplayName()
                })
                .ToList();
        }

        private void PopulateTasks(Project Project)
        {
            if (Project.Id == 0) return;
            Tasks = _UnitOfWork.ProjectTask
                .GetAll(pt => pt.ProjectId == Project.Id)
                .OrderBy(ProjectId => ProjectId.StartDate)
                .ToList();
        }

        public IActionResult OnGet(int? id)
        {
            PopulateStatusOptions();
            if (id == null || id == 0)
            {
                // New
                Project = new Project { 
                    Title = "New Project",
                    Description = "Project description goes here...",
                    StatusEnum = ProjectStatus.NewlySubmitted 
                };
            }
            else
            {
                // Edit existing
                Project = _UnitOfWork.Project.GetById(id.Value);
                if (Project == null)
                    return NotFound();
                PopulateTasks(Project);
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                PopulateStatusOptions();
                PopulateTasks(Project);
                return Page();
            }

            if (Project.Id == 0)
            {
                _UnitOfWork.Project.Add(Project);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _UnitOfWork.ProjectUser.Add(new ProjectUser {
                    ProjectId = Project.Id,
                    ApplicationUserId = userId,
                    CanAddTasks = true,
                    CanEditTasks = true,
                    CanRemoveTasks = true,
                    CanAddFiles = true,
                    CanEditFiles = true,
                    CanRemoveFiles = true,
                    CanSendMessages = true,
                    CanEditStatus = true,
                    Role = 0,
                });
            }
            else
            {
                _UnitOfWork.Project.Update(Project);
            }
                
            //_UnitOfWork.SaveChanges();  // save changes
            return RedirectToPage("./Index");
        }

        public IActionResult OnPostUpdateTask([FromBody] GanttTaskUpdateModel update)
        {
            var task = _UnitOfWork.ProjectTask.GetById(update.Id);
            if (task == null) return NotFound();
            task.StartDate = DateTime.Parse(update.Start);
            task.EndDate = DateTime.Parse(update.End);
            _UnitOfWork.ProjectTask.Update(task);
            var dto = new ProjectTaskDto
            {
                id = task.Id.ToString(),
                projectId = task.ProjectId,
                title = task.Title,
                description = task.Description,
                startDate = task.StartDate?.ToString("yyy-MM-dd") ?? "",
                endDate = task.EndDate?.ToString("yyy-MM-dd") ?? "",
                isScheduled = task.IsScheduled,
                isActive = task.IsActive,
                isCompleted = task.IsCompleted,
                statusDisplay = task.StatusDisplay,
                statusBadgeClass = task.StatusBadgeClass
            };
            return new JsonResult(new {
                success = true,
                task = dto,
                task_complete = task.IsCompleted 
            });
        }
    }

    public class ProjectTaskDto
    {
        public string id { get; set; }
        public int projectId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public bool isScheduled { get; set; }
        public bool isActive { get; set; }
        public bool isCompleted { get; set; }
        public string statusDisplay { get; set; }
        public string statusBadgeClass { get; set; }
    }

    public class GanttTaskUpdateModel
    {
        public int Id { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }

}
