using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace API.Helpers
{
    public class AzureBlobStorageSettings
    {
        public string Uri { get; set; } = default!;
        public string Account { get; set; } = default!;
        public string Key { get; set; } = default!;
        public string ConnectionString { get; set; } = default!;
        public string ContainerName { get; set; } = default!;


        public static implicit operator AzureBlobStorageSettings(BlobServiceClient b)
        {
            throw new NotImplementedException();
        }
    }
}