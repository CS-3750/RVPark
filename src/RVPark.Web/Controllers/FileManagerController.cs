using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RVPark.Application;
using RVPark.Core.Interfaces;
using RVPark.Core.Models;
using System.Security.Claims;

namespace RVPark.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private readonly IS3Service _s3Service;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FileManagerController(IS3Service s3Service, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _s3Service = s3Service;
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("upload-url")]
        public async Task<IActionResult> GetUploadUrl([FromBody] FileUploadRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Check project access
            var hasAccess = await _context.ProjectUsers
                .AnyAsync(pu => pu.ApplicationUserId == userId && pu.ProjectId == request.ProjectId);

            if (!hasAccess)
                return Forbid("You don't have access to this project");

            try
            {
                // Get next version number
                var existingVersions = await _context.Files
                    .Where(f => f.Name == request.FileName)
                    .Join(_context.ProjectFiles, f => f.Id, pf => pf.FileId, (f, pf) => new { f.Version, pf.ProjectId })
                    .Where(x => x.ProjectId == request.ProjectId)
                    .Select(x => x.Version)
                    .ToListAsync();

                var nextVersion = existingVersions.Any() ? existingVersions.Max() + 1 : 1;

                var uploadUrl = await _s3Service.GenerateUploadPresignedUrlAsync(
                    request.FileName, request.ProjectId.ToString(), nextVersion);

                return Ok(new
                {
                    uploadUrl,
                    version = nextVersion,
                    s3Key = _s3Service.GenerateS3Key(request.FileName, request.ProjectId.ToString(), nextVersion)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating upload URL: {ex.Message}");
            }
        }

        [HttpPost("confirm-upload")]
        public async Task<IActionResult> ConfirmUpload([FromBody] FileUploadConfirmRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var appUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
                if (appUser == null) return BadRequest("User not found");

                // Mark previous versions as not latest
                var existingFiles = await _context.Files
                    .Where(f => f.Name == request.FileName && f.IsLatestVersion)
                    .Join(_context.ProjectFiles, f => f.Id, pf => pf.FileId, (f, pf) => new { File = f, pf.ProjectId })
                    .Where(x => x.ProjectId == request.ProjectId)
                    .Select(x => x.File)
                    .ToListAsync();

                foreach (var file in existingFiles)
                {
                    file.IsLatestVersion = false;
                }

                // Create new file record
                var newFile = new Core.Models.File
                {
                    Name = request.FileName,
                    Type = Path.GetExtension(request.FileName),
                    Url = $"s3://{request.S3Key}",
                    UploadedAt = DateTime.UtcNow,
                    CreatedByApplicationUserId = appUser.Id,
                    Version = request.Version,
                    IsLatestVersion = true,
                    FileSizeBytes = request.FileSizeBytes,
                    VersionDescription = request.VersionDescription
                };

                // Set parent file if this is a new version
                if (request.Version > 1 && existingFiles.Any())
                {
                    var parentFile = existingFiles.First();
                    newFile.ParentFileId = parentFile.ParentFileId ?? parentFile.Id;
                }

                _context.Files.Add(newFile);
                await _context.SaveChangesAsync();

                // Create project-file relationship
                var projectFile = new ProjectFile
                {
                    ProjectId = request.ProjectId,
                    FileId = newFile.Id
                };

                _context.ProjectFiles.Add(projectFile);
                await _context.SaveChangesAsync();

                return Ok(new { fileId = newFile.Id, version = newFile.Version });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error confirming upload: {ex.Message}");
            }
        }

        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> GetDownloadUrl(int fileId, int? version = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var file = await _context.Files.FindAsync(fileId);
                if (file == null) return NotFound("File not found");

                // Check project access
                var projectFile = await _context.ProjectFiles.FirstOrDefaultAsync(pf => pf.FileId == fileId);
                if (projectFile == null) return NotFound("File not associated with any project");

                var hasAccess = await _context.ProjectUsers
                    .AnyAsync(pu => pu.ApplicationUserId == userId && pu.ProjectId == projectFile.ProjectId);

                if (!hasAccess) return Forbid("You don't have access to this project");

                // If specific version requested, find that version
                if (version.HasValue && version.Value != file.Version)
                {
                    var versionedFile = await _context.Files
                        .Where(f => f.Name == file.Name && f.Version == version.Value)
                        .Join(_context.ProjectFiles, f => f.Id, pf => pf.FileId, (f, pf) => new { File = f, pf.ProjectId })
                        .Where(x => x.ProjectId == projectFile.ProjectId)
                        .Select(x => x.File)
                        .FirstOrDefaultAsync();

                    if (versionedFile != null) file = versionedFile;
                }

                var downloadUrl = await _s3Service.GenerateDownloadPresignedUrlAsync(
                    file.Name, projectFile.ProjectId.ToString(), file.Version);

                return Ok(new
                {
                    downloadUrl,
                    fileName = file.Name,
                    version = file.Version,
                    fileSize = file.FileSizeDisplay
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating download URL: {ex.Message}");
            }
        }

        [HttpGet("project/{projectId}/files")]
        public async Task<IActionResult> GetProjectFiles(int projectId, bool latestOnly = true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Check project access
            var hasAccess = await _context.ProjectUsers
                .AnyAsync(pu => pu.ApplicationUserId == userId && pu.ProjectId == projectId);

            if (!hasAccess) return Forbid("You don't have access to this project");

            try
            {
                var query = _context.ProjectFiles
                    .Where(pf => pf.ProjectId == projectId)
                    .Join(_context.Files, pf => pf.FileId, f => f.Id, (pf, f) => f);

                if (latestOnly)
                {
                    query = query.Where(f => f.IsLatestVersion);
                }

                var filesWithUsers = await query
                    .Select(f => new
                    {
                        f.Id,
                        f.Name,
                        f.Type,
                        f.UploadedAt,
                        f.Version,
                        f.IsLatestVersion,
                        f.FileSizeBytes,
                        f.VersionDescription,
                        f.CreatedByApplicationUserId
                    })
                    .ToListAsync();

                var userIds = filesWithUsers.Select(f => f.CreatedByApplicationUserId).Distinct().ToList();
                var users = await _context.ApplicationUsers
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.UserName);

                var result = filesWithUsers.Select(f => new
                {
                    f.Id,
                    f.Name,
                    f.Type,
                    f.UploadedAt,
                    f.Version,
                    f.IsLatestVersion,
                    FileSizeDisplay = f.FileSizeBytes.HasValue ? FormatFileSize(f.FileSizeBytes.Value) : "Unknown",
                    f.VersionDescription,
                    UploadedBy = users.GetValueOrDefault(f.CreatedByApplicationUserId, "Unknown")
                }).OrderBy(f => f.Name).ThenByDescending(f => f.Version);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving files: {ex.Message}");
            }
        }

        [HttpGet("file/{fileId}/versions")]
        public async Task<IActionResult> GetFileVersions(int fileId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var file = await _context.Files.FindAsync(fileId);
                if (file == null) return NotFound("File not found");

                // Check project access
                var projectFile = await _context.ProjectFiles.FirstOrDefaultAsync(pf => pf.FileId == fileId);
                if (projectFile == null) return NotFound("File not associated with any project");

                var hasAccess = await _context.ProjectUsers
                    .AnyAsync(pu => pu.ApplicationUserId == userId && pu.ProjectId == projectFile.ProjectId);

                if (!hasAccess) return Forbid("You don't have access to this project");

                // Get all versions of this file
                var allVersions = await _context.Files
                    .Where(f => f.Name == file.Name)
                    .Join(_context.ProjectFiles, f => f.Id, pf => pf.FileId, (f, pf) => new { File = f, pf.ProjectId })
                    .Where(x => x.ProjectId == projectFile.ProjectId)
                    .Select(x => x.File)
                    .OrderByDescending(f => f.Version)
                    .ToListAsync();

                var userIds = allVersions.Select(f => f.CreatedByApplicationUserId).Distinct().ToList();
                var users = await _context.ApplicationUsers
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.UserName);

                var result = allVersions.Select(f => new
                {
                    f.Id,
                    f.Version,
                    f.UploadedAt,
                    FileSizeDisplay = f.FileSizeBytes.HasValue ? FormatFileSize(f.FileSizeBytes.Value) : "Unknown",
                    f.VersionDescription,
                    f.IsLatestVersion,
                    UploadedBy = users.GetValueOrDefault(f.CreatedByApplicationUserId, "Unknown")
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving file versions: {ex.Message}");
            }
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    public class FileUploadRequest
    {
        public string FileName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
    }

    public class FileUploadConfirmRequest
    {
        public string FileName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public int Version { get; set; }
        public string S3Key { get; set; } = string.Empty;
        public long? FileSizeBytes { get; set; }
        public string? VersionDescription { get; set; }
    }
}