using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
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
    }
}
