using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RVPark.Application;
using RVPark.Core.Models;
using RVPark.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RVPark.Pages.Notes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context) => _context = context;

        [BindProperty]
        [Required]
        public string Title { get; set; }

        [BindProperty]
        [Required]
        public string Content { get; set; }

        public List<Note> Notes { get; set; }

        public void OnGet()
        {
            Notes = _context.Notes
                .OrderByDescending(n => n.Created)
                .ToList();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Notes = _context.Notes
                    .OrderByDescending(n => n.Created)
                    .ToList();
                return Page();
            }

            var note = new Note
            {
                Title = Title,
                Content = Content,
                Created = DateTime.UtcNow
            };

            _context.Notes.Add(note);
            _context.SaveChanges();

            return RedirectToPage(); // reloads and shows new note
        }
    }
}