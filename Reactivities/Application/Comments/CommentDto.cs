using System;

namespace Application.Comments
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Image { get; set; }
    }
}