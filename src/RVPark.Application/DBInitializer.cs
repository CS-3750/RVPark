using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RVPark.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Application
{
    public class DBInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DBInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            _db.Database.EnsureCreated();

            // Migrations if they are not applied
            try
            {
                if(_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            // Catch if the db has already been worked on
            if (_db.Roles.Any(r => r.Name == SD.AdminRole))
            {
                return; // DB has been seeded
            }

            // Seed roles
            _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.InternRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.ClientRole)).GetAwaiter().GetResult();

            string adminEmail = "admin@sharklasers.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "AdminUser",
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    PhoneNumber = "1234567890",
                };
                var createResult = await _userManager.CreateAsync(adminUser, "TestPassword123!");
                if (!createResult.Succeeded)
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
            if (!await _userManager.IsInRoleAsync(adminUser, SD.AdminRole))
            {
                await _userManager.AddToRoleAsync(adminUser, SD.AdminRole);
            }
        }
    }
}
