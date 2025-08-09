using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RVPark.Application;
using RVPark.Core.Models;
using System.Linq;

namespace RVPark.Web.Pages.Shared;

public class MessagesModel(UnitOfWork _unitOfWork) : PageModel
{
    [BindProperty]
    public Message NewMessage { get; set; } = new Message { };
    public List<Message> Messages { get; set; } = [];
    public List<ApplicationUser> Users { get; set; } = [];
    public List<Project> UserProjects { get; set; } = [];
    public Project CurrentProject { get; set; }

    public void OnGet(int? projectId)
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            return;

        var userId = claim.Value;

        UserProjects = _unitOfWork.ProjectUser
            .GetAll(pu => pu.ApplicationUserId == userId, includes: "Project")
            .Select(pu => pu.Project)
            .OrderBy(p => p.Name)
            .ToList();

        if (UserProjects.Any())
        {
            var currentProjectId = projectId ?? UserProjects.First().Id;
            CurrentProject = UserProjects.FirstOrDefault(p => p.Id == currentProjectId);

            if (CurrentProject != null)
            {
                var projectUserIds = _unitOfWork.ProjectUser
                    .GetAll(pu => pu.ProjectId == CurrentProject.Id)
                    .Select(pu => pu.ApplicationUserId)
                    .ToList();

                Users = _unitOfWork.User
                    .GetAll(u => u.Id != userId && projectUserIds.Contains(u.Id))
                    .ToList();

                Messages = _unitOfWork.Message
                    .GetAll(m => m.ProjectId == CurrentProject.Id && (m.ReceiverId == userId || m.SenderId == userId),
                        includes: "Sender,Receiver")
                    .ToList();
            }
        }
    }

    public IActionResult OnPost()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            return Page();
        
        NewMessage.SenderId = claim.Value;
        NewMessage.CreatedAt = DateTime.UtcNow;
        _unitOfWork.Message.Add(NewMessage);
        return RedirectToPage(new { projectId = NewMessage.ProjectId });
    }
}
