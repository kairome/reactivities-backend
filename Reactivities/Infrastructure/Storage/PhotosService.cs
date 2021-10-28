using System.Threading.Tasks;
using Application.Core;
using Application.Storage;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Storage
{
    public class PhotosService : IPhotosService
    {
        private readonly ICloudinaryAccount _cloudinary;

        public PhotosService(ICloudinaryAccount cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            if (file == null)
            {
                throw new BadRequest("File cannot be empty");
            }

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(200).Height(200).Crop("fill"),
            };

            var uploadResult = await _cloudinary.Account.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new BadRequest("Failed to upload the image", uploadResult.Error.Message);
            }

            return new PhotoUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.ToString()
            };
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.Account.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                throw new BadRequest("Failed to delete photo", result.Error.Message);
            }

            return result.Result;
        }
    }
}