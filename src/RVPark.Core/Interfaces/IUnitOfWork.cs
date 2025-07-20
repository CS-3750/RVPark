using RVPark.Core.Models;
using File = RVPark.Core.Models.File;

namespace RVPark.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<AssetRequest> AssetRequest { get; }
        IGenericRepository<File> File { get; }
        IGenericRepository<Message> Message { get; }
        IGenericRepository<Payment> Payment { get; }
        IGenericRepository<Project> Project { get; }
        IGenericRepository<ProjectFile> ProjectFile { get; }
        IGenericRepository<ProjectProposal> ProjectProposal { get; }
        IGenericRepository<ProjectProposalFile> ProjectProposalFile { get; }
        IGenericRepository<ProjectTask> ProjectTask { get; }
        IGenericRepository<ProjectUser> ProjectUser { get; }
        IGenericRepository<ProjectUserHoursLog> ProjectUserHoursLog { get; }
        IGenericRepository<ApplicationUser> User { get; }
    }
}