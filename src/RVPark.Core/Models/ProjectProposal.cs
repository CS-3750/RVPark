using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class ProjectProposal
    {
        [Key]
        public int Id { get; set; }

        public int? ProjectId { get; set; }

        [MaxLength(255), Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        [MaxLength(64)]
        public string ContactFirstName { get; set; }
        [MaxLength(64)]
        public string ContactLastName { get; set; }
        [MaxLength(255)]
        public string ContactEmail { get; set; }
        [MaxLength(16)]
        public string ContactPhone { get; set; }

        public DateTime? RequestedCompletionDate { get; set; }
        [MaxLength(255)]
        public string CompanyName { get; set; }
        public decimal? Budget { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
    }
}
