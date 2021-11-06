using System;
using System.Text.Json.Serialization;

namespace Domain
{
    public class ActivityNotification
    {
        public string Id { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ActivityId { get; set; }
        public string ActivityTitle { get; set; }
        public ActivityNotificationType Type { get; set; }
    }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActivityNotificationType
    {
        Edited,
        Deleted,
        Cancelled,
        Activated,
        NewMessages,
    }
}