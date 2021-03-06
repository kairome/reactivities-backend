using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class User : IDDocument
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public Photo ProfilePhoto { get; set; }
        public string Bio { get; set; }
        public List<Photo> Photos { get; set; } = new();
        public List<ActivityNotification> Notifications { get; set; } = new();
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Photo ProfilePhoto { get; set; }
        public List<Photo> Photos { get; set; }
        public string Bio { get; set; }
        public List<ActivityNotification> Notifications { get; set; }
        
        public UserDto(User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            DisplayName = user.DisplayName;
            Email = user.Email;
            ProfilePhoto = user.ProfilePhoto;
            Photos = user.Photos;
            Bio = user.Bio;
            Notifications = user.Notifications.OrderBy(x => x.IsRead)
                .ThenByDescending(x => x.CreatedAt).ToList();
        }
    }

    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
    }
}