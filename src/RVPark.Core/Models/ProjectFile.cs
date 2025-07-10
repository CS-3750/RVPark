using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class ProjectFile
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int FileId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        [ForeignKey(nameof(FileId))]
        public File File { get; set; }
    }
}
