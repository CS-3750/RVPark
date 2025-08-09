using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RVPark.Application;
using RVPark.Core.Models;

namespace RVPark.Web.Pages.Shared;

public class MessagesModel(UnitOfWork _unitOfWork) : PageModel
{
    [BindProperty]
    public Message NewMessage { get; set; } = new Message { };
    public List<Message> Messages { get; set; } = [];
    public List<ApplicationUser> Users { get; set; } = [];
    
    public void OnGet()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            return;

        Users = _unitOfWork.User.GetAll().ToList();
        
        Messages = _unitOfWork.Message
            .GetAll(m => m.ReceiverId == claim.Value
                              || m.SenderId == claim.Value,
                includes:"Sender,Receiver")
            .ToList();
    }

    public IActionResult OnPost()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            return RedirectToPage();
        
        NewMessage.SenderId = claim.Value;
        _unitOfWork.Message.Add(NewMessage);
        return RedirectToPage();
    }
}