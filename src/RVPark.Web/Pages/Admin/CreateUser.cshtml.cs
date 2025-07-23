using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RVPark.Core.Models;
using RVPark.Core.Interfaces;
using RVPark.Application;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RVPark.Web.Pages.Admin
{
    [Authorize(Roles = SD.AdminRole)]
    public class CreateUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public CreateUserModel(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [BindProperty]
        public CreateUserViewModel Input { get; set; } = new();

        public List<SelectListItem> RoleOptions { get; set; } = new();
        public List<SelectListItem> ProjectOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadSelectListsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "A user with this email already exists.");
                await LoadSelectListsAsync();
                return Page();
            }

            // Create ApplicationUser with extended fields
            var appUser = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                CreatedDate = DateTime.Now,
                IsActive = Input.IsActive,
                EmailConfirmed = true
            };

            // Add to DbContext and save
            _context.ApplicationUsers.Add(appUser);
            await _context.SaveChangesAsync();

            // Generate temporary password and hash it
            var tempPassword = GenerateRandomPassword();
            var passwordHasher = new PasswordHasher<IdentityUser>();
            appUser.PasswordHash = passwordHasher.HashPassword(appUser, tempPassword);
            
            // Update the user with hashed password
            await _context.SaveChangesAsync();

            // Add user to role using IdentityUser for role management
            if (!string.IsNullOrEmpty(Input.Role))
            {
                var identityUser = await _userManager.FindByIdAsync(appUser.Id);
                if (identityUser != null)
                {
                    await _userManager.AddToRoleAsync(identityUser, Input.Role);
                }
            }

            // Assign to projects if selected
            if (Input.SelectedProjectIds?.Any() == true)
            {
                foreach (var projectId in Input.SelectedProjectIds)
                {
                    var projectUser = new ProjectUser
                    {
                        ProjectId = projectId,
                        ApplicationUserId = appUser.Id,
                        Role = GetProjectRoleFromSystemRole(Input.Role),
                        Admin = Input.Role == SD.AdminRole,
                        CanAddTasks = Input.Role != SD.ClientRole,
                        CanEditTasks = Input.Role != SD.ClientRole,
                        CanRemoveTasks = Input.Role == SD.AdminRole,
                        CanAddFiles = true,
                        CanEditFiles = Input.Role != SD.ClientRole,
                        CanRemoveFiles = Input.Role == SD.AdminRole,
                        CanSendMessages = true,
                        CanEditStatus = Input.Role == SD.AdminRole
                    };

                    _unitOfWork.ProjectUser.Add(projectUser);
                }
            }

            TempData["SuccessMessage"] = $"User created successfully! Temporary password: {tempPassword}";
            return RedirectToPage("./EditUser");
        }

        private async Task LoadSelectListsAsync()
        {
            // Load role options
            RoleOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = SD.AdminRole, Text = "Admin" },
                new SelectListItem { Value = SD.InternRole, Text = "Intern" },
                new SelectListItem { Value = SD.ClientRole, Text = "Client" }
            };

            // Load project options
            var projects = await _unitOfWork.Project.GetAllAsync();
            ProjectOptions = projects.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Title
            }).ToList();
        }

        private static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static int GetProjectRoleFromSystemRole(string systemRole)
        {
            return systemRole switch
            {
                SD.AdminRole => 2, // Admin role in project
                SD.InternRole => 1, // Intern/contributor role
                SD.ClientRole => 0, // Client/viewer role
                _ => 0
            };
        }
    }

    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Assign to Projects")]
        public List<int> SelectedProjectIds { get; set; } = new();
    }
}