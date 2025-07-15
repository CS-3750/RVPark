using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using BB.Application; // adjust based on your namespace

namespace BB.Web.Controllers
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

            project.Status = "Pending";
            _unitOfWork.Project.Add(project);
            return Ok(new { success = true, message = "Project created successfully." });
        }

        // 2. Assign interns to a project
        [HttpPost("{projectId}/assign-interns")]
        public IActionResult AssignInterns(int projectId, [FromBody] List<int> internIds)
        {
            var project = _unitOfWork.Project.GetById(projectId);
            if (project == null)
                return NotFound();

            foreach (var internId in internIds)
            {
                var assignment = new InternAssignment
                {
                    ProjectId = projectId,
                    InternId = internId,
                    AssignedDate = DateTime.UtcNow
                };
                _unitOfWork.InternAssignment.Add(assignment);
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
        public IActionResult UpdateProjectStatus(int id, [FromBody] string status)
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

            // Optionally include intern assignments, logs, etc.
            var interns = _unitOfWork.InternAssignment.GetByProjectId(id);
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

            project.Status = "Approved";
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

            project.Status = "Denied";
            _unitOfWork.Project.Update(project);
            return Ok(new { success = true, message = "Project denied." });
        }
    }
}