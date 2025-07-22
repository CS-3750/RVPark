using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class ProjectTask
    {
        [Key] 
        public int Id { get; set; }

        public int ProjectId { get; set; }

        [MaxLength(128), Required]
        public string Title { get; set; }

        [Required] 
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [ValidateNever]
        [ForeignKey(nameof(ProjectId))] 
        public Project Project { get; set; }

        [NotMapped]
        public bool IsScheduled => StartDate.HasValue && StartDate.Value > DateTime.Now;

        [NotMapped]
        public bool IsActive => (StartDate.HasValue && StartDate.Value <= DateTime.Now && (!EndDate.HasValue || EndDate.Value >= DateTime.Now));

        [NotMapped]
        public bool IsCompleted => EndDate.HasValue && EndDate.Value < DateTime.Now;

        [NotMapped]
        public string StatusDisplay
            => IsCompleted ? "Completed"
             : IsActive ? "Active"
             : IsScheduled ? "Scheduled"
             : "Unknown";

        [NotMapped]
        public string StatusBadgeClass
            => IsCompleted ? "bg-success"
             : IsActive ? "bg-primary"
             : /*scheduled*/ "bg-secondary";
    }
}
