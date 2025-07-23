using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RVPark.Application;
using RVPark.Core.Models;

namespace BB.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public UserController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. User login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _unitOfWork.User.Get(u => u.Email == login.Email);
            if (user == null || !VerifyPassword(login.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials." });

            // Generate JWT or session here (implementation depends on your auth setup)
            return Ok(new { success = true, message = "Login successful." });
        }

        // 2. User registration
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel register)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Use Get with predicate to check for existing email
            if (_unitOfWork.User.Get(u => u.Email == register.Email) != null)
                return BadRequest(new { message = "Email already exists." });

            // Use ApplicationUser instead of User
            var user = new ApplicationUser
            {
                UserName = register.Email,
                Email = register.Email,
                FirstName = register.Name, // Assuming Name maps to FirstName
                LastName = "", // Set as needed
                PasswordHash = HashPassword(register.Password),
                IsActive = true // Set default as needed
            };
            _unitOfWork.User.Add(user);
            return Ok(new { success = true, message = "Registration successful." });
        }

        // 3. Update user profile (admin only)
        [HttpPut("{id}/updateprofile")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProfile(int id, [FromBody] ApplicationUser updatedUser)
        {
            var user = _unitOfWork.User.GetById(id);
            if (user == null)
                return NotFound();

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.IsActive = updatedUser.IsActive;
            // Add other fields as needed

            _unitOfWork.User.Update(user);
            return Ok(new { success = true, message = "Profile updated successfully." });
        }

        // 4. Manage user roles (admin only)
        [HttpPut("{id}/manageroles")]
        [Authorize(Roles = "Admin")]
        public IActionResult ManageRoles(int id, [FromBody] string role)
        {
            var user = _unitOfWork.User.GetById(id);
            if (user == null)
                return NotFound();

            // Assuming roles are managed via claims or a separate property/table.
            // Example: Add a custom property or use ASP.NET Core Identity role management.
            // Here, you could set a custom claim, or if you have a UserRoles table, update it accordingly.
            // For demonstration, let's assume you have a method to set the role (pseudo-code):

            // Example: user.SetRole(role); // Implement this method as needed
            // Or, if using Identity, update roles via UserManager (not shown here)

            // Since ApplicationUser does not have a Role property, you need to implement role management elsewhere.
            // For now, return a NotImplemented result.
            return StatusCode(501, new { success = false, message = "Role management not implemented. Please implement role assignment logic." });
        }

        // Helper methods for password hashing/verification
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }

    // Example models for login and registration
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}