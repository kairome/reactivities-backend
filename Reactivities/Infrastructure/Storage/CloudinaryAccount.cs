using AspNetCore.ServiceRegistration.Dynamic;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage
{
    [ScopedService]
    public interface ICloudinaryAccount
    {
        Cloudinary Account { get; }
    }
    
    public class CloudinaryAccount : ICloudinaryAccount
    {
        public Cloudinary Account { get; }

        public CloudinaryAccount(IOptions<CloudinarySettings> opts)
        {
            var account = new Account(opts.Value.CloudName, opts.Value.ApiKey, opts.Value.ApiSecret);
            Account = new Cloudinary(account);
        }
    }
}