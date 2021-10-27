using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Auth;
using AspNetCore.ServiceRegistration.Dynamic;
using Domain;
using MongoDB.Driver;
using Persistence;

namespace Application.Users
{
    [ScopedService]
    public interface IWriteUsersService : IWriteDbOperationsService<User>
    {
        Task<User> CreateUser(NewUserDto dto);
        Task UpdateUserToken(string userId, string token);
        Task<User> AddProfilePhoto(string userId, Photo photo);
        Task<User> DeletePhoto(string userId, string photoId);
        Task<User> UpdateProfilePhoto(string userId, Photo photo);
        Task<User> UpdateUserInfo(string userId, UpdateUserDto dto);
    }

    public class WriteUsersService : WriteDbOperationsService<User>, IWriteUsersService
    {
        public WriteUsersService(IMongoContext mongoContext) : base(mongoContext, "Users")
        {
        }

        public async Task<User> CreateUser(NewUserDto dto)
        {
            var (hash, salt) = PasswordService.GenerateSaltHash(dto.Password);
            return await InsertOne(new User
            {
                Id = IdGenerator.GetNewId(),
                UserName = dto.UserName,
                Hash = hash,
                Salt = salt,
                DisplayName = dto.DisplayName,
                Email = dto.Email,
            });
        }

        public async Task UpdateUserToken(string userId, string token)
        {
            await UpdateOneById(userId, Update.Set(x => x.Token, token));
        }

        public async Task<User> AddProfilePhoto(string userId, Photo photo)
        {
            return await UpdateOneById(userId, Update.Set(x => x.ProfilePhoto, photo).Push(x => x.Photos, photo));
        }

        public async Task<User> DeletePhoto(string userId, string photoId)
        {
            return await UpdateOneById(userId, Update.PullFilter(x => x.Photos, p => p.Id == photoId));
        }

        public async Task<User> UpdateProfilePhoto(string userId, Photo photo)
        {
            return await UpdateOneById(userId, Update.Set(x => x.ProfilePhoto, photo));
        }

        public async Task<User> UpdateUserInfo(string userId, UpdateUserDto dto)
        {
            return await UpdateOneById(userId,
                Update
                    .Set(x => x.DisplayName, dto.Name)
                    .Set(x => x.Bio, dto.Bio)
                    .Set(x => x.Email, dto.Email)
            );
        }
    }
}