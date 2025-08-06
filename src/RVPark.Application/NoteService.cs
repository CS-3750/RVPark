using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RVPark.Core.Interfaces;
using RVPark.Core.Models;

namespace RVPark.Application
{
    public class NotesService : INotesService
    {
        private readonly ApplicationDbContext _db;

        public NotesService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Note>> GetAllNotesAsync()
        {
            return await _db.Notes
                .OrderByDescending(n => n.Created)
                .ToListAsync();
        }

        public async Task<Note> AddNoteAsync(Note note)
        {
            _db.Notes.Add(note);
            await _db.SaveChangesAsync();
            return note;
        }

        public async Task<Note> GetNoteByIdAsync(int id)
        {
            return await _db.Notes.FindAsync(id);
        }
    }
}