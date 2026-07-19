using CloudinaryDotNet;
using Freelify.Models.Enums;

namespace Freelify.Services
{
    public class FileUploadService
    {
        private readonly Cloudinary _cloudinary;
        public FileUploadService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);

        }

        private string _GetPublicIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.Segments;
            var publicIdWithExtension = segments.Last();
            var publicId = publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));
            return publicId;
        }
        public async Task<string> UploadFile(IFormFile file, UploadFileType? fileType = null)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("No document uploaded.");


            string prefix = fileType switch
            {
                UploadFileType.Image => "image",
                UploadFileType.Video => "video",
                UploadFileType.PDF => "application/pdf",
                _ => ""
            };

            if (fileType != null && !file.ContentType.Contains(prefix))
            {
                throw new ArgumentException($"File is not of correct format.");
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = fileType switch
                {
                    UploadFileType.Video => new CloudinaryDotNet.Actions.VideoUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                    },
                    UploadFileType.PDF => new CloudinaryDotNet.Actions.RawUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                    },
                    _ => new CloudinaryDotNet.Actions.ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                    },
                };



                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"File upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();

            }
        }

        public async Task<IList<string>> UploadFiles(IList<IFormFile> files)
        {
            var uploadTasks = files.Select(file => UploadFile(file));

            return (await Task.WhenAll(uploadTasks)).ToList();
        }


        public async Task DeleteFile(string? url)
        {
            if (string.IsNullOrEmpty(url) || !url.Contains("res.cloudinary.com")) return;

            var publicId = _GetPublicIdFromUrl(url);
            var deletionParams = new CloudinaryDotNet.Actions.DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deletionParams);
        }
    }
}
