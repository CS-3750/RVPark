using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class AssetRequest
    {
        [Key] 
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int CreatedByUserId { get; set; }

        [Required] 
        public string Description { get; set; }
        public int Quantity { get; set; }

        [MaxLength(1024), Required]
        public string Url { get; set; }

        [Required] 
        public string EstimatedCost { get; set; }
        public int Status { get; set; }

        [ForeignKey(nameof(ProjectId))] 
        public Project Project { get; set; }

        [ForeignKey(nameof(CreatedByUserId))] 
        public User User { get; set; }
    }
}
