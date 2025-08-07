using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RVPark.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Application
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<ProjectUserHoursLog> ProjectUserHourLogs { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<Core.Models.File> Files { get; set; }
        public DbSet<ProjectFile> ProjectFiles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<AssetRequest> AssetRequests { get; set; }
        public DbSet<ProjectProposal> ProjectProposals { get; set; }
        public DbSet<ProjectProposalFile> ProjectProposalFiles { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ProjectNote> ProjectNotes { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
