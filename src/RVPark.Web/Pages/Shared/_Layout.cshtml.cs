using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RVPark.Application;
using RVPark.Core.Models;

namespace RVPark.Web.Pages.Shared;

public class LayoutModel(UnitOfWork unitOfWork): PageModel
{
    // public List<Project> ProjectList { get; set; }
    public void OnGet()
    {
        ViewData["Thing"] = true;
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            ViewData["UserFound"] = true;
            ViewData["ProjectList"] = unitOfWork.ProjectUser
                .GetAll(p => p.ApplicationUserId == claim.Value, includes:"Project")
                .Select(pu => pu.Project)
                .ToList();
        }
        else {
            ViewData["UserFound"] = false;
        }
    }
}