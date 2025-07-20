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

        public void Initialize()
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

            // Seed admin user
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName="AdminUser",
                Email="admin@sharklasers.com",
                FirstName="Admin",
                LastName="User",
                PhoneNumber="1234567890",
            }, "TestPassword!").GetAwaiter().GetResult();
            ApplicationUser adminUser = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@sharklasers.com");
            _userManager.AddToRoleAsync(adminUser, SD.AdminRole).GetAwaiter().GetResult();
        }
    }
}
