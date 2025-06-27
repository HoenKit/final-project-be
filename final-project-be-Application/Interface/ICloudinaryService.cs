using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface ICloudinaryService
    {
        public Task<VideoUploadResult> UploadVideoStreamAsync(Stream fileStream, string fileName);
        public Task<string> UploadVideoAndGetUrlAsync(IFormFile videoFile);
        public Task<bool> DeleteVideoByUrlAsync(string videoUrl);
    }
}
