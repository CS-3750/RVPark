using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace RVPark.Core.Models
{
    public class ProjectNote
    {
        public int ProjectId { get; set; }

        [ValidateNever]
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}