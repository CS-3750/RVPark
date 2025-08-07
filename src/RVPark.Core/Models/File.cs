using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Models
{
    public class File
    {
        [Key] 
        public int Id { get; set; }

        [MaxLength(255), Required] 
        public string Name { get; set; }
        
        [MaxLength(100), Required]
        public string Type { get; set; }
        
        [MaxLength(1024), Required]
        public string Url { get; set; }

        public DateTime UploadedAt { get; set; }
        public string CreatedByApplicationUserId { get; set; }

        // Versioning fields
        public int Version { get; set; } = 1;
        public int? ParentFileId { get; set; }
        public bool IsLatestVersion { get; set; } = true;
        public long? FileSizeBytes { get; set; }
        
        [MaxLength(500)]
        public string? VersionDescription { get; set; }

        [ForeignKey(nameof(CreatedByApplicationUserId))] 
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey(nameof(ParentFileId))]
        public File? ParentFile { get; set; }

        public ICollection<File> ChildVersions { get; set; } = new List<File>();

        [NotMapped]
        public string DisplayName => $"{Name} (v{Version})";

        [NotMapped]
        public string FileSizeDisplay => FileSizeBytes.HasValue ? FormatFileSize(FileSizeBytes.Value) : "Unknown";

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
}
