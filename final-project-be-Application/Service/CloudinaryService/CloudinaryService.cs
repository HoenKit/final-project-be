using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using final_project_be_Application.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace final_project_be_Application.Service.CloudinaryService
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
		private readonly ILogger<CloudinaryService> _logger;

		public CloudinaryService(IOptions<CloudinarySettings> config, ILogger<CloudinaryService> logger)
		{
			var account = new Account(
				config.Value.CloudName,
				config.Value.ApiKey,
				config.Value.ApiSecret
			);
			_cloudinary = new Cloudinary(account);
			_logger = logger;
		}

		public async Task<VideoUploadResult> UploadVideoStreamAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
                PublicId = "videos/" + Guid.NewGuid().ToString("N"),
                EagerTransforms = new List<Transformation>()
            {
                new EagerTransformation().Width(300).Height(300).Crop("pad").AudioCodec("none"),
                new EagerTransformation().Width(160).Height(100).Crop("crop").Gravity("south").AudioCodec("none")
            },
                EagerAsync = true
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }
		public async Task<string> UploadVideoAndGetUrlAsync(IFormFile videoFile)
		{
			if (videoFile == null || videoFile.Length == 0)
				throw new ArgumentException("Invalid video file");

			await using var stream = videoFile.OpenReadStream();

			var uploadResult = await UploadVideoStreamAsync(stream, videoFile.FileName);

			if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
			{
				return uploadResult.SecureUrl.ToString();
			}

			throw new Exception($"Video upload failed with status: {uploadResult.StatusCode}");
		}

		public async Task<bool> DeleteVideoByUrlAsync(string videoUrl)
		{
			try
			{
				var match = Regex.Match(videoUrl, @"upload/.+?/(.+)\.mp4");
				if (!match.Success)
					return false;

				var publicId = match.Groups[1].Value;

				var deletionParams = new DeletionParams(publicId)
				{
					ResourceType = ResourceType.Video
				};

				var result = await _cloudinary.DestroyAsync(deletionParams);

				return result.Result == "ok" || result.Result == "not_found";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when delete video");
				return false;
			}
		}

	}

}
