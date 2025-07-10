using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class Message
    {
        [Key] 
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int UserId { get; set; }

        [Required] 
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool EmailsSent { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
