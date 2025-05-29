using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace final_project_be_Application.Service.CloudinaryService
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

		public CloudinaryService(IOptions<CloudinarySettings> config)
		{
			var account = new Account(
				config.Value.CloudName,
				config.Value.ApiKey,
				config.Value.ApiSecret
			);
			_cloudinary = new Cloudinary(account);
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
    }

}
