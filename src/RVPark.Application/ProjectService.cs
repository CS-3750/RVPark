using RVPark.Core.Interfaces;
using RVPark.Core.Models;

namespace RVPark.Application;

public class ProjectService(IUnitOfWork unitOfWork) : IProjectService
{
    public async Task<List<Project>> GetUserProjectsAsync(string userId)
    {
        return await Task.Run(() =>
            unitOfWork.ProjectUser
                .GetAll(pu => pu.ApplicationUserId == userId, includes: "Project")
                .Select(pu => pu.Project)
                .ToList()
        );
    }
}