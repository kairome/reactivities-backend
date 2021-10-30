using System;
using System.Collections.Generic;

namespace Domain
{
    public class Activity : IDDocument
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public bool IsCancelled { get; set; }

        public List<Comment> Comments { get; set; } = new();
        public List<ActivityAttendee> Attendees { get; set; } = new();
    }

    public class ActivityAttendee
    {
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}