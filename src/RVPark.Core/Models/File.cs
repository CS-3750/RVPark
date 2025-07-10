using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class File
    {
        [Key] 
        public int Id { get; set; }

        [MaxLength(255), Required] 
        public string Name { get; set; }
        
        [MaxLength(100), Required]
        public string Type { get; set; }
        
        [MaxLength(1024), Required]
        public string Url { get; set; }

        public DateTime UploadedAt { get; set; }
        public int CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))] 
        public User User { get; set; }
    }
}
