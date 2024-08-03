using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace API.Helpers
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }

        internal ImageUploadResult UploadAsync(ImageUploadParams uploadParams)
        {
            throw new NotImplementedException();
        }

        public static implicit operator CloudinarySettings(Cloudinary v)
        {
            throw new NotImplementedException();
        }
    }
}