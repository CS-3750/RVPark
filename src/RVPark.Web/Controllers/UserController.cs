using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BB.Application;

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
            var user = _unitOfWork.User.GetByEmail(login.Email);
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

            if (_unitOfWork.User.GetByEmail(register.Email) != null)
                return BadRequest(new { message = "Email already exists." });

            var user = new User
            {
                Name = register.Name,
                Email = register.Email,
                PasswordHash = HashPassword(register.Password),
                Role = "User"
            };
            _unitOfWork.User.Add(user);
            return Ok(new { success = true, message = "Registration successful." });
        }

        // 3. Update user profile (admin only)
        [HttpPut("{id}/updateprofile")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProfile(int id, [FromBody] User updatedUser)
        {
            var user = _unitOfWork.User.GetById(id);
            if (user == null)
                return NotFound();

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
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

            user.Role = role;
            _unitOfWork.User.Update(user);
            return Ok(new { success = true, message = "Role updated successfully." });
        }

        // Helper methods for password hashing/verification (implement as needed)
        private string HashPassword(string password)
        {
            // Implement password hashing logic
            return password; // Placeholder
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // Implement password verification logic
            return password == passwordHash; // Placeholder
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