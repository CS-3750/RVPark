using System.Collections.Generic;
using System.Threading.Tasks;
using RVPark.Core.Models;

namespace RVPark.Core.Interfaces
{
    public interface INotesService
    {
        Task<List<Note>> GetAllNotesAsync();
        Task<Note> AddNoteAsync(Note note);
        Task<Note> GetNoteByIdAsync(int id);
        // Add more as needed!
    }
}