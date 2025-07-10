using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidOn { get; set; }

        [MaxLength(1026), Required]
        public string StripeLink { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
    }
}
