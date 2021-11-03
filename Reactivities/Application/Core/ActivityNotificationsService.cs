using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Users;
using AspNetCore.ServiceRegistration.Dynamic;
using Domain;

namespace Application.Core
{
    [ScopedService]
    public interface IActivityNotificationsService
    {
        Task SendActivityUpdateMessage(Activity activity, ActivityNotificationType type);
    }

    public class ActivityNotificationsService : IActivityNotificationsService
    {
        private readonly INotificationService _notificationService;
        private readonly IWriteUsersService _writeUsersService;
        private readonly IGetUsersService _getUsersService;

        public ActivityNotificationsService(
            INotificationService notificationService,
            IWriteUsersService writeUsersService,
            IGetUsersService getUsersService
        )
        {
            _notificationService = notificationService;
            _writeUsersService = writeUsersService;
            _getUsersService = getUsersService;
        }

        public async Task SendActivityUpdateMessage(Activity activity, ActivityNotificationType type)
        {
            var followerIds = activity.Followers.Select(x => x.UserId).ToList();
            var users = await _getUsersService.GetUsersByIds(followerIds);

            foreach (var followerId in followerIds)
            {
                var user = users.FirstOrDefault(x => x.Id == followerId);
                var hasMsgNotification = user?.Notifications.FirstOrDefault(x =>
                    x.ActivityId == activity.Id && !x.IsRead && x.Type == ActivityNotificationType.NewMessages);

                if (hasMsgNotification == null || type != ActivityNotificationType.NewMessages)
                {
                    var userNotification = new ActivityNotification
                    {
                        Id = IdGenerator.GetNewId(),
                        ActivityId = activity.Id,
                        ActivityTitle = activity.Title,
                        Type = type,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    };

                    await _writeUsersService.AddNotification(followerId, userNotification);
                    await _notificationService.SendMessage(followerId, "activityUpdated", userNotification);
                }
            }
        }
    }
}