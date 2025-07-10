using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class ProjectProposalFile
    {
        [Key] public int Id { get; set; }

        public int ProjectProposalId { get; set; }
        public int FileId { get; set; }

        [ForeignKey(nameof(ProjectProposalId))]
        public ProjectProposal ProjectProposal { get; set; }

        [ForeignKey(nameof(FileId))]
        public File File { get; set; }
    }
}
