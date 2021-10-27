using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Activities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
    public class ActivityAuthorRequirement : IAuthorizationRequirement
    {
    }

    public class ActivityAuthorRequirementHandler : AuthorizationHandler<ActivityAuthorRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGetActivitiesService _getActivitiesService;

        public ActivityAuthorRequirementHandler(
            IHttpContextAccessor contextAccessor,
            IGetActivitiesService getActivitiesService
        )
        {
            _contextAccessor = contextAccessor;
            _getActivitiesService = getActivitiesService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActivityAuthorRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Task.CompletedTask;
            }

            var activityId = _contextAccessor.HttpContext?.Request.RouteValues.SingleOrDefault(x => x.Key == "id")
                .Value?.ToString();

            if (string.IsNullOrEmpty(activityId))
            {
                return Task.CompletedTask;
            }

            var activity = _getActivitiesService.GetOneById(activityId).Result;

            if (activity == null)
            {
                return Task.CompletedTask;
            }

            if (activity.AuthorId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}