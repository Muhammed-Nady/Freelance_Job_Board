using CloudinaryDotNet;
using Freelify.Models.Enums;

namespace Freelify.Services
{
    public class FileUploadService
    {
        private readonly Cloudinary _cloudinary;
        private readonly object _fileTypePrefixes;
        public FileUploadService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);

            _fileTypePrefixes = new Dictionary<string, string>
            {
                { "image", "image" },
                { "video", "video" },
                { "pdf", "application/pdf" }
            };
        }

        public async Task<string> UploadFile(IFormFile file, UploadFileType fileType)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("No document uploaded.");


            string prefix = fileType switch
            {
                UploadFileType.Image => "image",
                UploadFileType.Video => "video",
                UploadFileType.PDF => "application/pdf",
                _ => throw new ArgumentException("Unsupported file type.")
            };

            if (!file.ContentType.Contains(prefix))
            {
                throw new ArgumentException($"File is not of correct format.");
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                };

                if (fileType == UploadFileType.Video || fileType == UploadFileType.Image)
                    uploadParams.Transformation = new Transformation().Quality("auto").FetchFormat("auto");


                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"File upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();

            }
        }

    }
}
