using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class ProjectUserHoursLog
    {
        [Key]
        public int Id { get; set; }

        public int ProjectUserId { get; set; }
        public int Hours { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey(nameof(ProjectUserId))] 
        public ProjectUser ProjectUser { get; set; }
    }
}
