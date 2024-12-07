using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        public AzureBlobService(IOptions<AzureBlobStorageSettings> configuration)
        {
            _blobServiceClient = new BlobServiceClient(new Uri(configuration.Value.Uri), new StorageSharedKeyCredential(configuration.Value.Account, configuration.Value.Key));
            _containerName = configuration.Value.ContainerName;
        }
        // public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        // {
        //     string sanitizedFileName = SanitizeFileName(fileName);

        //     var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        //     var blobClient = containerClient.GetBlobClient(sanitizedFileName);
        //     await blobClient.UploadAsync(fileStream, overwrite: true);

        //     return blobClient.Uri.ToString();
        // }
        // private string SanitizeFileName(string fileName)
        // {
        //     // Remove invalid characters
        //     string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        //     string sanitizedName = new string(fileName
        //         .Where(x => !invalidChars.Contains(x))
        //         .ToArray());

        //     // Truncate to max length (1024 characters for Azure Blob Storage)
        //     sanitizedName = sanitizedName.Length > 1024
        //         ? sanitizedName.Substring(0, 1024)
        //         : sanitizedName;

        //     // Ensure non-empty filename
        //     return string.IsNullOrWhiteSpace(sanitizedName)
        //         ? Guid.NewGuid().ToString()
        //         : sanitizedName;
        // }
        public async Task<string> UploadFileAsync(Stream fileStream, string originalFileName)
        {
            // Generate a unique filename to avoid conflicts
            string uniqueFileName = $"{Guid.NewGuid()}_{SanitizeFileName(originalFileName)}";

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

                // Ensure container exists
                await containerClient.CreateIfNotExistsAsync();

                var blobClient = containerClient.GetBlobClient(uniqueFileName);
                await blobClient.UploadAsync(fileStream, overwrite: true);

                return blobClient.Uri.ToString();
            }
            catch (RequestFailedException ex)
            {
                // Log the detailed error
                Console.WriteLine($"Upload failed: {ex.Message}");
                throw;
            }
        }

        private string SanitizeFileName(string fileName)
        {
            // Remove invalid characters and limit length
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            string sanitizedName = new string(fileName
                .Where(x => !invalidChars.Contains(x))
                .Take(200) // Limit to 200 characters
                .ToArray());

            return string.IsNullOrWhiteSpace(sanitizedName)
                ? "unnamed_file"
                : sanitizedName;
        }
    }
}