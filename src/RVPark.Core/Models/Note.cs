using System;
using System.ComponentModel.DataAnnotations;

namespace RVPark.Core.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        // For future use:
        // public int? ProjectId { get; set; }
        // public Project Project { get; set; }
        // public string UserId { get; set; }
    }
}