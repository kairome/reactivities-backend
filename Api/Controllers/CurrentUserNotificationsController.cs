using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Users;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/api/current-user/notifications")]
    public class CurrentUserNotificationsController : BaseController
    {
        private readonly IWriteUsersService _writeUsersService;
        private readonly ICurrentUserIdentity _currentUserIdentity;

        public CurrentUserNotificationsController(IWriteUsersService writeUsersService, ICurrentUserIdentity currentUserIdentity)
        {
            _writeUsersService = writeUsersService;
            _currentUserIdentity = currentUserIdentity;
        }
        
        [HttpPut("{id}/read")]
        public async Task<UserDto> MarkNotificationAsRead(string id)
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            var updatedUser = await _writeUsersService.MarkNotificationAsRead(currentUser.Id, id);

            return new UserDto(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<UserDto> RemoveNotification(string id)
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            var updatedUser = await _writeUsersService.RemoveNotification(currentUser.Id, id);
            return new UserDto(updatedUser);
        }
        
        [HttpDelete]
        public async Task<UserDto> ClearAllNotification()
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            var updatedUser = await _writeUsersService.ClearAllNotifications(currentUser.Id);
            return new UserDto(updatedUser);
        }
    }
}