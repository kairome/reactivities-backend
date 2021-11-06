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
        Task AddNotification(string userId, ActivityNotification notification);
        Task<User> MarkNotificationAsRead(string userId, string notificationId);
        Task<User> RemoveNotification(string userId, string notificationId);
        Task<User> ClearAllNotifications(string userId);
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

        public async Task AddNotification(string userId, ActivityNotification notification)
        {
            await UpdateOneById(userId, Update.Push(x => x.Notifications, notification));
        }

        public async Task<User> MarkNotificationAsRead(string userId, string notificationId)
        {
            return await UpdateOne(
                Filter.Eq(x => x.Id, userId) & Filter.ElemMatch(x => x.Notifications, n => n.Id == notificationId),
                Update.Set(x => x.Notifications[-1].IsRead, true));
        }

        public async Task<User> RemoveNotification(string userId, string notificationId)
        {
            return await UpdateOneById(userId, Update.PullFilter(x => x.Notifications, n => n.Id == notificationId));
        }

        public async Task<User> ClearAllNotifications(string userId)
        {
            return await UpdateOneById(userId, Update.Set(x => x.Notifications, new List<ActivityNotification>()));
        }
    }
}