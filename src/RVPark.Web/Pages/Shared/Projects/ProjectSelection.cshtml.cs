using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RVPark.Core.Models;
using RVPark.Application;

namespace RVPark.Web.Pages.Shared.Projects;

public class ProjectSelection(UnitOfWork unitOfWork) : PageModel
{
    public List<Project> ProjectList { get; set; }

    public void OnGet()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            ProjectList = unitOfWork.ProjectUser
                .GetAll(p => p.ApplicationUserId == claim.Value, includes:"Project")
                .Select(pu => pu.Project)
                .ToList();
        }
    }
}