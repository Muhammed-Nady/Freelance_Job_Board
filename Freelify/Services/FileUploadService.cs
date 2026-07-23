using CloudinaryDotNet;
using Freelify.Models.Enums;
using Freelify.Models.Results;


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

        public async Task<FileUploadResult> UploadFile(IFormFile file, UploadFileType? fileType = null)
        {


            if (file == null || file.Length == 0)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorType = ErrorType.BadRequest,
                    ErrorMessage = "No document uploaded."
                };
            }

            if (fileType == null)
            {
                if (file.ContentType.StartsWith("image"))
                {
                    fileType = UploadFileType.Image;
                }
                else
                {
                    fileType = UploadFileType.PDF;
                }
            }

            //string prefix = fileType switch
            //{
            //    UploadFileType.Image => "image",
            //    UploadFileType.Video => "video",
            //    UploadFileType.PDF => "application/pdf",
            //    _ => ""
            //};

            //if (fileType != null && !file.ContentType.Contains(prefix))
            //{
            //    return new FileUploadResult
            //    {
            //        Success = false,
            //        ErrorType = ErrorType.BadRequest,
            //        ErrorMessage = $"File is not of correct format. Expected: {prefix}."
            //    };
            //}

            try
            {
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
                        return new FileUploadResult
                        {
                            Success = false,
                            ErrorType = ErrorType.BadRequest,
                            ErrorMessage = $"File upload failed: {uploadResult.Error.Message}"
                        };
                    }

                    return new FileUploadResult
                    {
                        Success = true,
                        Url = uploadResult.SecureUrl.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorType = ErrorType.BadRequest,
                    ErrorMessage = $"An error occurred during file upload: {ex.Message}"
                };
            }
        }

        public async Task<IList<FileUploadResult>> UploadFiles(IList<IFormFile> files)
        {
            var uploadTasks = files.Select(file => UploadFile(file));
            var results = await Task.WhenAll(uploadTasks);
            return results.ToList();
        }

        public async Task DeleteFile(string url)
        {
            if (string.IsNullOrEmpty(url) || !url.Contains("res.cloudinary.com")) return;

            try
            {
                var publicId = _GetPublicIdFromUrl(url);
                var deletionParams = new CloudinaryDotNet.Actions.DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deletionParams);
            }
            catch
            {
                // Silently ignore deletion failures
            }
        }
    }
}
