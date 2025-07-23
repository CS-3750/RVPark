using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RVPark.Core.Models;
using RVPark.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RVPark.Web.Pages.Shared.Projects;

public class AssetRequestModel(IUnitOfWork unitOfWork) : PageModel
{
    [BindProperty]
    public AssetRequest Input { get; set; }

    public List<AssetRequest> PreviousRequests { get; set; }
    
    public int ProjectId { get; set; }

    public void OnGet(int projectId)
    {        
        ProjectId = projectId;
        PreviousRequests = unitOfWork.AssetRequest.GetAll(
            ar => ar.ProjectId == projectId, null, "").ToList();

        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        Input = new AssetRequest
        {
            ProjectId = projectId, 
            CreatedByApplicationUserId = currentUserId,
            StatusEnum = AssetRequestStatus.Submitted
        };
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                var field = error.Key; 
                var messages = error.Value.Errors.Select(e => e.ErrorMessage); 

                Console.WriteLine($"Field: {field}, Errors: {string.Join(", ", messages)}");
            }

        }

        var newRequest = new AssetRequest
        {
            ProjectId = Input.ProjectId,
            CreatedByApplicationUserId = Input.CreatedByApplicationUserId,
            Description = Input.Description,
            Quantity = Input.Quantity,
            Url = Input.Url,
            EstimatedCost = Input.EstimatedCost,
            StatusEnum = AssetRequestStatus.Submitted
        };

        unitOfWork.AssetRequest.Add(newRequest);
        TempData["SuccessMessage"] = "Asset request created successfully.";

        return RedirectToPage(new { projectId = Input.ProjectId });
    }
    
    public IActionResult OnPostUpdateStatus(int requestId, int status)
    {
        var requestToUpdate = unitOfWork.AssetRequest.Get(r => r.Id == requestId);
        if (requestToUpdate != null)
        {
            requestToUpdate.Status = status;
            unitOfWork.AssetRequest.Update(requestToUpdate);
            TempData["SuccessMessage"] = "Asset request status updated successfully.";
        }

        return RedirectToPage(new { projectId = requestToUpdate.ProjectId });
    }
    
    public IActionResult OnPostDelete(int requestId)
    {
        var requestToDelete = unitOfWork.AssetRequest.Get(r => r.Id == requestId);
        if (requestToDelete != null)
        {
            var projectId = requestToDelete.ProjectId;
            unitOfWork.AssetRequest.Delete(requestToDelete);
            TempData["SuccessMessage"] = "Asset request deleted successfully.";
            return RedirectToPage(new { projectId = projectId });
        }
        return Page();
    }
}