using System.Collections.Generic;
using System.Threading.Tasks;
using RVPark.Core.Models;

namespace RVPark.Core.Interfaces
{
    public interface IProjectNotesService
    {
        Task<List<ProjectNote>> GetAllProjectNotesAsync();
        Task<ProjectNote> AddProjectNoteAsync(ProjectNote note);
        Task<ProjectNote> GetProjectNoteByIdAsync(int id);
        // Add more as needed!
    }
}