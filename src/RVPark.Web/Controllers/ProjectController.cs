using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using BB.Application; // adjust based on your namespace

namespace BB.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public ProjectController(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = unitOfWork;
            _WebHostEnvironment = webHostEnvironment;
        }

        // GET: api/project
        [HttpGet]
        public IActionResult Get()
        {
            var allProjects = _UnitOfWork.Project.GetAll(); // Add includes if needed
            return Json(new { data = allProjects });
        }

        // GET: api/project/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var project = _UnitOfWork.Project.GetById(id);
            if (project == null)
                return NotFound();

            return Ok(project);
        }

        // POST: api/project
        [HttpPost]
        public IActionResult Create([FromBody] Project project)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _UnitOfWork.Project.Add(project);
            return Ok(new { success = true, message = "Project created successfully." });
        }

        // PUT: api/project/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Project updatedProject)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = _UnitOfWork.Project.GetById(id);
            if (project == null)
                return NotFound();

            // update properties manually
            project.Name = updatedProject.Name;
            project.Description = updatedProject.Description;
            // add more fields as necessary

            _UnitOfWork.Project.Update(project);
            return Ok(new { success = true, message = "Project updated successfully." });
        }

        // DELETE: api/project/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var project = _UnitOfWork.Project.GetById(id);
            if (project == null)
                return Json(new { success = false, message = "Error while deleting" });

            _UnitOfWork.Project.Delete(project);
            return Json(new { success = true, message = "Delete successful" });
        }
    }
}
