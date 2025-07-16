using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using RVPark.Application;
using RVPark.Core.Models;

namespace RVPark.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProjectController(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        // 1. Create a new project
        [HttpPost("create")]
        public IActionResult CreateProject([FromBody] Project project)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            project.Status = 0; // Assuming 0 represents "Pending" status
            _unitOfWork.Project.Add(project);
            return Ok(new { success = true, message = "Project created successfully." });
        }

        // 2. Assign interns to a project
        [HttpPost("{projectId}/assign-interns")]
        public IActionResult AssignInterns(int projectId, [FromBody] List<string> internUserIds)
        {
            var project = _unitOfWork.Project.GetById(projectId);
            if (project == null)
                return NotFound();

            foreach (var internUserId in internUserIds)
            {
                var projectUser = new ProjectUser
                {
                    ProjectId = projectId,
                    ApplicationUserId = internUserId,
                    CanAddTasks = false,
                    CanAddFiles = false,
                    Role = 0 // Set appropriate role for intern
                };
                _unitOfWork.ProjectUser.Add(projectUser);
            }
            return Ok(new { success = true, message = "Interns assigned successfully." });
        }

        // 3. Update project details
        [HttpPut("{id}/update")]
        public IActionResult UpdateProject(int id, [FromBody] Project updatedProject)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = _unitOfWork.Project.GetById(id);
            if (project == null)
                return NotFound();

            project.Name = updatedProject.Name;
            project.Description = updatedProject.Description;
            // Add other fields as needed

            _unitOfWork.Project.Update(project);
            return Ok(new { success = true, message = "Project updated successfully." });
        }

        // 4. Update project status (e.g., Pending, Approved, Denied, Completed)
        [HttpPut("{id}/status")]
        public IActionResult UpdateProjectStatus(int id, [FromBody] int status)
        {
            var project = _unitOfWork.Project.GetById(id);
            if (project == null)
                return NotFound();

            project.Status = status;
            _unitOfWork.Project.Update(project);
            return Ok(new { success = true, message = "Project status updated." });
        }

        // 5. Get project details
        [HttpGet("{id}/details")]
        public IActionResult GetProjectDetails(int id)
        {
            var project = _unitOfWork.Project.GetById(id);
            if (project == null)
                return NotFound();

            // Get assigned interns (ProjectUser with Role == 0, assuming 0 is intern)
            var interns = _unitOfWork.ProjectUser.GetAll(
                pu => pu.ProjectId == id && pu.Role == 0,
                null,
                "ApplicationUser"
            ).ToList();

            return Ok(new { project, interns });
        }

        // 6. Delete a project
        [HttpDelete("{id}/delete")]
        public IActionResult DeleteProject(int id)
        {
            var project = _unitOfWork.Project.GetById(id);
            if (project == null)
                return Json(new { success = false, message = "Error while deleting" });

            _unitOfWork.Project.Delete(project);
            return Json(new { success = true, message = "Delete successful" });
        }

        // 7. Approve a project (engineer/admin action)
        [HttpPost("{id}/approve")]
        public IActionResult ApproveProject(int id)
        {
            var project = _unitOfWork.Project.GetById(id);
            if (project == null)
                return NotFound();

            project.Status = 1; // Assuming 1 represents "Approved" status
            _unitOfWork.Project.Update(project);
            return Ok(new { success = true, message = "Project approved." });
        }

        // 8. (Optional) Deny a project
        [HttpPost("{id}/deny")]
        public IActionResult DenyProject(int id)
        {
            var project = _unitOfWork.Project.GetById(id);
            if (project == null)
                return NotFound();

            project.Status = 2; // Assuming 2 represents "Denied" status
            _unitOfWork.Project.Update(project);
            return Ok(new { success = true, message = "Project denied." });
        }
    }
}