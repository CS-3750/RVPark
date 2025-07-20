using RVPark.Core.Models;

namespace RVPark.Core.Interfaces;

public interface IProjectService
{
    Task<List<Project>> GetUserProjectsAsync(string userId);
}