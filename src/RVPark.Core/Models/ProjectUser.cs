using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class ProjectUser
    {
        [Key] 
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public string ApplicationUserId { get; set; }

        public bool Admin { get; set; }
        public bool CanAddTasks { get; set; }
        public bool CanEditTasks { get; set; }
        public bool CanRemoveTasks { get; set; }
        public bool CanAddFiles { get; set; }
        public bool CanEditFiles { get; set; }
        public bool CanRemoveFiles { get; set; }
        public bool CanSendMessages { get; set; }
        public bool CanEditStatus { get; set; }
        public int Role { get; set; }

        [ForeignKey(nameof(ProjectId))] 
        public Project Project { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
