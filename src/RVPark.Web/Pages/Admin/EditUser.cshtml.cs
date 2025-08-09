using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RVPark.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using RVPark.Core.Interfaces;
using RVPark.Application;
using Microsoft.EntityFrameworkCore;

namespace RVPark.Application.Pages.Admin
{
    [Authorize(Roles = SD.AdminRole)]
    public class EditUserModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EditUserModel(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
        }

        public IEnumerable<UserViewModel> Users { get; set; }

        [BindProperty]
        public UserEditDto EditInput { get; set; } = new();

        public List<SelectListItem> RoleOptions { get; set; } = new();
        public List<SelectListItem> ProjectOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadUsersAsync();
            await LoadSelectListsAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _unitOfWork.User.GetAsync(u => u.Id == id);
            if (user != null)
            {
                // Remove user from all projects first
                var projectUsers = await _unitOfWork.ProjectUser.GetAllAsync(pu => pu.ApplicationUserId == id);
                foreach (var projectUser in projectUsers)
                {
                    _unitOfWork.ProjectUser.Delete(projectUser);
                }
                
                _unitOfWork.User.Delete(user);
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditUserAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadUsersAsync();
                await LoadSelectListsAsync();
                return Page();
            }

            var user = await _unitOfWork.User.GetAsync(u => u.Id == EditInput.Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage();
            }

            // Update user details
            user.FirstName = EditInput.FirstName;
            user.LastName = EditInput.LastName;
            user.Email = EditInput.Email;
            user.UserName = EditInput.Email;
            user.IsActive = EditInput.IsActive;

            _unitOfWork.User.Update(user);

            // Update role if changed - use ApplicationUser for role management
            var identityUser = await _userManager.FindByIdAsync(user.Id);
            if (identityUser != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(identityUser);
                if (!currentRoles.Contains(EditInput.Role))
                {
                    if (currentRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);
                    }
                    await _userManager.AddToRoleAsync(identityUser, EditInput.Role);
                }
            }

            // Update project assignments
            var existingProjectUsers = await _unitOfWork.ProjectUser.GetAllAsync(pu => pu.ApplicationUserId == user.Id);
            
            // Remove old assignments
            foreach (var existingPU in existingProjectUsers)
            {
                _unitOfWork.ProjectUser.Delete(existingPU);
            }

            // Add new assignments
            if (EditInput.SelectedProjectIds?.Any() == true)
            {
                foreach (var projectId in EditInput.SelectedProjectIds)
                {
                    var projectUser = new ProjectUser
                    {
                        ProjectId = projectId,
                        ApplicationUserId = user.Id,
                        Role = GetProjectRoleFromSystemRole(EditInput.Role),
                        Admin = EditInput.Role == SD.AdminRole,
                        CanAddTasks = EditInput.Role != SD.ClientRole,
                        CanEditTasks = EditInput.Role != SD.ClientRole,
                        CanRemoveTasks = EditInput.Role == SD.AdminRole,
                        CanAddFiles = true,
                        CanEditFiles = EditInput.Role != SD.ClientRole,
                        CanRemoveFiles = EditInput.Role == SD.AdminRole,
                        CanSendMessages = true,
                        CanEditStatus = EditInput.Role == SD.AdminRole
                    };

                    _unitOfWork.ProjectUser.Add(projectUser);
                }
            }

            TempData["SuccessMessage"] = "User updated successfully.";

            return RedirectToPage();
        }

        private async Task LoadUsersAsync()
        {
            var userEntities = await _unitOfWork.User.GetAllAsync();
            var usersWithRoles = new List<UserViewModel>();

            foreach (var user in userEntities)
            {
                var identityUser = await _userManager.FindByIdAsync(user.Id);
                var roles = identityUser != null ? await _userManager.GetRolesAsync(identityUser) : new List<string>();
                var projectUsers = await _unitOfWork.ProjectUser.GetAllAsync(pu => pu.ApplicationUserId == user.Id);
                var projects = new List<Project>();
                
                foreach (var pu in projectUsers)
                {
                    var project = await _unitOfWork.Project.GetAsync(p => p.Id == pu.ProjectId);
                    if (project != null) projects.Add(project);
                }

                usersWithRoles.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = roles.FirstOrDefault() ?? "None",
                    IsActive = user.IsActive,
                    AssignedProjects = projects.Select(p => p.Title).ToList(),
                    AssignedProjectIds = projects.Select(p => p.Id).ToList()
                });
            }

            Users = usersWithRoles;
        }

        private async Task LoadSelectListsAsync()
        {
            RoleOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = SD.AdminRole, Text = "Admin" },
                new SelectListItem { Value = SD.InternRole, Text = "Intern" },
                new SelectListItem { Value = SD.ClientRole, Text = "Client" }
            };

            var projects = await _unitOfWork.Project.GetAllAsync();
            ProjectOptions = projects.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Title
            }).ToList();
        }

        private static int GetProjectRoleFromSystemRole(string systemRole)
        {
            return systemRole switch
            {
                SD.AdminRole => 2,
                SD.InternRole => 1,
                SD.ClientRole => 0,
                _ => 0
            };
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public List<string> AssignedProjects { get; set; } = new();
        public List<int> AssignedProjectIds { get; set; } = new();
    }

    public class UserEditDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public List<int> SelectedProjectIds { get; set; } = new();
    }
}