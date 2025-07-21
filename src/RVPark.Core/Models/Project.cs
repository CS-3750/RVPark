using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RVPark.Core.Utilities;

namespace RVPark.Core.Models
{
    public enum ProjectStatus
    {
        [Display(Name = "Newly Submitted")]
        NewlySubmitted = 0,

        [Display(Name = "Active")]
        Active = 1,

        [Display(Name = "Denied")]
        Denied = 2,

        [Display(Name = "Reviewing")]
        Reviewing = 3,

        [Display(Name = "Completed")]
        Completed = 3
    }
    
    public class Project
    {
        [Key] 
        public int Id { get; set; }

        [MaxLength(255), Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EstimatedEndDate { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public ProjectStatus StatusEnum
        {
            get => (ProjectStatus)Status;
            set => Status = (int)value;
        }

        [NotMapped]
        public string StatusDisplay => StatusEnum.GetDisplayName();

        [NotMapped]
        public bool IsActive => StatusEnum == ProjectStatus.Active;
    }
}
