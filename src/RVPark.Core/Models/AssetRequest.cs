using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RVPark.Core.Utilities;

namespace RVPark.Core.Models
{
    public enum AssetRequestStatus
    {
        [Display(Name = "Submitted")]
        Submitted = 0,

        [Display(Name = "Approved")]
        Approved = 1,

        [Display(Name = "Denied")]
        Denied = 2,
        
        [Display(Name = "Ordered")]
        Ordered = 3,
        
        [Display(Name = "Received")]
        Received = 4
    }

    public class AssetRequest
    {
        [Key] 
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public string CreatedByApplicationUserId { get; set; }

        [Required] 
        public string Description { get; set; }
        public int Quantity { get; set; }

        [MaxLength(1024), Required]
        public string Url { get; set; }

        [Required] 
        public string EstimatedCost { get; set; }
        public int Status { get; set; }
        
        [NotMapped]
        public AssetRequestStatus StatusEnum
        {
            get => (AssetRequestStatus)Status;
            set => Status = (int)value;
        }

        [NotMapped]
        public string StatusDisplay => StatusEnum.GetDisplayName();

        [ForeignKey(nameof(ProjectId))] 
        public Project Project { get; set; }

        [ForeignKey(nameof(CreatedByApplicationUserId))] 
        public ApplicationUser ApplicationUser { get; set; }
    }
}
