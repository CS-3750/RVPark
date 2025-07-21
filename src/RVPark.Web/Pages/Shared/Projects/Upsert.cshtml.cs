using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RVPark.Application;
using RVPark.Core.Models;
using RVPark.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace RVPark.Web.Pages.Shared.Projects
{
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _UnitOfWork;

        [BindProperty]
        public Project Project { get; set; }

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
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                PopulateStatusOptions();
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
    }
}
