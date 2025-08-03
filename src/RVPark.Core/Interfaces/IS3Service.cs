using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Interfaces
{
    public interface IS3Service
    {
        Task<string> GenerateUploadPresignedUrlAsync(string fileName, string projectId, int version = 1);
        Task<string> GenerateDownloadPresignedUrlAsync(string fileName, string projectId, int version = 1);
        Task<bool> DeleteFileAsync(string fileName, string projectId, int version = 1);
        Task<bool> FileExistsAsync(string fileName, string projectId, int version = 1);
        Task<List<int>> GetFileVersionsAsync(string fileName, string projectId);
        string GenerateS3Key(string fileName, string projectId, int version);
    }
}