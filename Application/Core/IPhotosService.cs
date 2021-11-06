using System.Threading.Tasks;
using Application.Storage;
using AspNetCore.ServiceRegistration.Dynamic;
using Microsoft.AspNetCore.Http;

namespace Application.Core
{
   [ScopedService]
    public interface IPhotosService
    {
        Task<PhotoUploadResult> AddPhoto(IFormFile file);
        Task<string> DeletePhoto(string publicId);
    }
}