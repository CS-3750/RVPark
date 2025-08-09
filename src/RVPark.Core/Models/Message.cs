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
        
        [Required]
        public string SenderId { get; set; }
        
        [Required]
        public string ReceiverId { get; set; }

        [Required, MaxLength(5000)] 
        public string Content { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        [ForeignKey(nameof(SenderId))]
        public ApplicationUser Sender { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public ApplicationUser Receiver { get; set; }
        
        public int ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool EmailsSent { get; set; }
    }
}
