
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace final_project_be.Ultils
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "phronesisfiles";

        // Dictionary to hold content types
        private readonly Dictionary<string, string> _contentTypes = new Dictionary<string, string>
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".pdf", "application/pdf" }
    };

        public BlobStorageService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task UploadFileAsync(string fileName, Stream fileStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(fileName);

            // Determine content type from file extension
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            string contentType = _contentTypes.ContainsKey(extension) ? _contentTypes[extension] : "application/octet-stream";

            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var downloadInfo = await blobClient.DownloadAsync();
            return downloadInfo.Value.Content;
        }

        //Delete file if exists
        public async Task DeleteFileIfExistsAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            // Check if the blob exists
            var exists = await blobClient.ExistsAsync();
            if (exists)
            {
                // Delete the blob if it exists
                await blobClient.DeleteAsync();
            }
        }
    }
}
