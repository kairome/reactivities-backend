namespace Domain
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Bio { get; set; }

        public UserProfile(User user)
        {
            Id = user.Id;
            Name = user.DisplayName;
            PhotoUrl = user.ProfilePhoto?.Url;
            Bio = user.Bio;
        }
    }

    public class UserProfileStats
    {
        public long ActivitiesHosting { get; set; }
        public long ActivitiesAttending { get; set; }
        public long ActivitiesFollowing { get; set; }
    }
}