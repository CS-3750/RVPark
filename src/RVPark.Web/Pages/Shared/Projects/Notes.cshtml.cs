using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RVPark.Application;
using RVPark.Core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RVPark.Web.Pages.Shared.Projects
{
    public class NotesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public NotesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ProjectId { get; set; }

        [BindProperty]
        [Required]
        public string Title { get; set; }

        [BindProperty]
        [Required]
        public string Content { get; set; }

        public List<ProjectNote> ProjectNotes { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ProjectNotes = await _context.ProjectNotes
                .Where(n => n.ProjectId == ProjectId)
                .OrderByDescending(n => n.Created)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ProjectNotes = await _context.ProjectNotes
                    .Where(n => n.ProjectId == ProjectId)
                    .OrderByDescending(n => n.Created)
                    .ToListAsync();

                return Page();
            }

            var note = new ProjectNote
            {
                Title = Title,
                Content = Content,
                ProjectId = ProjectId,
                Created = DateTime.UtcNow
            };

            _context.ProjectNotes.Add(note);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { projectId = ProjectId });
        }
    }
}