using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RVPark.Core.Interfaces;
using RVPark.Core.Models;

namespace RVPark.Application
{
    public class ProjectNotesService : IProjectNotesService
    {
        private readonly ApplicationDbContext _db;

        public ProjectNotesService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<ProjectNote>> GetAllProjectNotesAsync()
        {
            return await _db.ProjectNotes
                .OrderByDescending(n => n.Created)
                .ToListAsync();
        }

        public async Task<ProjectNote> AddProjectNoteAsync(ProjectNote note)
        {
            _db.ProjectNotes.Add(note);
            await _db.SaveChangesAsync();
            return note;
        }

        public async Task<ProjectNote> GetProjectNoteByIdAsync(int id)
        {
            return await _db.ProjectNotes.FindAsync(id);
        }
    }
}