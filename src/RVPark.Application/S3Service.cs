using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using RVPark.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Application
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:BucketName"] ?? throw new ArgumentNullException("AWS:BucketName configuration is missing");
        }

        public string GenerateS3Key(string fileName, string projectId, int version)
        {
            // Structure: projects/{projectId}/files/{sanitizedFileName}/v{version}/{fileName}
            var sanitizedFileName = SanitizeFileName(fileName);
            return $"projects/{projectId}/files/{sanitizedFileName}/v{version}/{fileName}";
        }

        public async Task<string> GenerateUploadPresignedUrlAsync(string fileName, string projectId, int version = 1)
        {
            var key = GenerateS3Key(fileName, projectId, version);
            
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(60), // URL expires in 1 hour
                ContentType = GetContentType(fileName)
            };

            return await _s3Client.GetPreSignedURLAsync(request);
        }

        public async Task<string> GenerateDownloadPresignedUrlAsync(string fileName, string projectId, int version = 1)
        {
            var key = GenerateS3Key(fileName, projectId, version);
            
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(60) // URL expires in 1 hour
            };

            return await _s3Client.GetPreSignedURLAsync(request);
        }

        public async Task<bool> DeleteFileAsync(string fileName, string projectId, int version = 1)
        {
            try
            {
                var key = GenerateS3Key(fileName, projectId, version);
                
                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(request);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> FileExistsAsync(string fileName, string projectId, int version = 1)
        {
            try
            {
                var key = GenerateS3Key(fileName, projectId, version);
                
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                await _s3Client.GetObjectMetadataAsync(request);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task<List<int>> GetFileVersionsAsync(string fileName, string projectId)
        {
            try
            {
                var sanitizedFileName = SanitizeFileName(fileName);
                var prefix = $"projects/{projectId}/files/{sanitizedFileName}/";
                
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = prefix
                };

                var response = await _s3Client.ListObjectsV2Async(request);
                var versions = new List<int>();

                foreach (var obj in response.S3Objects)
                {
                    // Extract version number from key: projects/{projectId}/files/{fileName}/v{version}/{fileName}
                    var keyParts = obj.Key.Split('/');
                    if (keyParts.Length >= 5 && keyParts[4].StartsWith("v"))
                    {
                        if (int.TryParse(keyParts[4].Substring(1), out int version))
                        {
                            versions.Add(version);
                        }
                    }
                }

                return versions.Distinct().OrderByDescending(v => v).ToList();
            }
            catch
            {
                return new List<int>();
            }
        }

        private string SanitizeFileName(string fileName)
        {
            // Remove file extension and sanitize for use in S3 key
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var sanitized = new string(nameWithoutExtension
                .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_')
                .ToArray());
            
            return string.IsNullOrWhiteSpace(sanitized) ? "file" : sanitized;
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                _ => "application/octet-stream"
            };
        }
    }
}