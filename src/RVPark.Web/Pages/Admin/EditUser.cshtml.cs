using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RVPark.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using RVPark.Core.Interfaces;

namespace RVPark.Application.Pages.Admin
{
    public class EditUserModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditUserModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<UserViewModel> Users { get; set; }

        public async Task OnGetAsync()
        {
            var userEntities = await _unitOfWork.User.GetAllAsync();
            Users = userEntities.Select(user => new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim()
            }).ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _unitOfWork.User.GetAsync(u => u.Id == id);
            if (user != null)
            {
                _unitOfWork.User.Delete(user);
            }
            return RedirectToPage();
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}