using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RVPark.Core.Models
{
    public class TimeEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public int ProjectTaskId { get; set; }

        [Required]
        [Range(0.25, 24.0, ErrorMessage = "Hours must be between 0.25 and 24")]
        public decimal Hours { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey(nameof(ProjectTaskId))]
        public ProjectTask ProjectTask { get; set; }

        [NotMapped]
        public string FormattedHours => Hours == 1 ? "1 hour" : $"{Hours:0.##} hours";

        [NotMapped]
        public string UserName => ApplicationUser?.UserName ?? "Unknown";

        [NotMapped]
        public string TaskTitle => ProjectTask?.Title ?? "Unknown Task";
    }
}