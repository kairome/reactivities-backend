using System.Threading.Tasks;
using Application.Activities;
using Application.Users;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/api/user-profile")]
    public class UserProfileController : BaseController
    {
        private readonly IGetUsersService _getUsersService;
        private readonly IGetActivitiesService _getActivitiesService;

        public UserProfileController(
            IGetUsersService getUsersService,
            IGetActivitiesService getActivitiesService
        )
        {
            _getUsersService = getUsersService;
            _getActivitiesService = getActivitiesService;
        }

        [HttpGet("{id}")]
        public async Task<UserProfile> GetUserProfile(string id)
        {
            var user = await _getUsersService.GetUserById(id);

            return new UserProfile(user);
        }

        [HttpGet("{id}/stats")]
        public async Task<UserProfileStats> GetUserStats(string id)
        {
            var myActivities = await _getActivitiesService.GetActivities(new ActivityFiltersDto
            {
                IsMy = true,
                Attending = true,
                Following = true,
            }, id);

            return new UserProfileStats
            {
                ActivitiesHosting = myActivities.FindAll(x => x.AuthorId == id).Count,
                ActivitiesAttending = myActivities.FindAll(x => x.Attendees.Exists(a => a.UserId == id)).Count,
                ActivitiesFollowing = myActivities.FindAll(x => x.Followers.Exists(f => f.UserId == id)).Count,
            };

        }
    }
}