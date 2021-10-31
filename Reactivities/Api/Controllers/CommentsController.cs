using System;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/api/activity/{activityId}/comments")]
    public class CommentsController : BaseController
    {
        private readonly ICurrentUserIdentity _currentUserIdentity;
        private readonly IWriteActivitiesService _writeActivitiesService;
        private readonly IChatService _chatService;

        public CommentsController(
            ICurrentUserIdentity currentUserIdentity,
            IWriteActivitiesService writeActivitiesService,
            IChatService chatService 
        )
        {
            _currentUserIdentity = currentUserIdentity;
            _writeActivitiesService = writeActivitiesService;
            _chatService = chatService;
        }

        [HttpPost]
        public async Task AddComment(string activityId, [FromBody] AddCommentDto dto)
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            var comment = new Comment
            {
                Id = IdGenerator.GetNewId(),
                AuthorId = currentUser.Id,
                Body = dto.Body,
                CreatedAt = DateTime.UtcNow
            };

            var commentDto = new CommentDto(comment, currentUser.DisplayName, currentUser.ProfilePhoto?.Url);
            await _writeActivitiesService.AddComment(activityId, comment);
            
            await _chatService.SendMessage(activityId, commentDto);
        }
    }
}