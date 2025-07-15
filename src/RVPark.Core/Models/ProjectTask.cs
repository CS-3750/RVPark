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

        [ForeignKey(nameof(ProjectId))] 
        public Project Project { get; set; }
    }
}
