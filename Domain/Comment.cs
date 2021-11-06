using System;

namespace Domain
{
    public class Comment
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommentDto : Comment
    {
        public string AuthorName { get; set; }
        public string AuthorProfileUrl { get; set; }

        public CommentDto(Comment comment, string name, string url)
        {
            Id = comment.Id;
            Body = comment.Body;
            AuthorId = comment.AuthorId;
            CreatedAt = comment.CreatedAt;
            AuthorName = name;
            AuthorProfileUrl = url;
        }
    }

    public class AddCommentDto
    {
        public string Body { get; set; }
    }
}