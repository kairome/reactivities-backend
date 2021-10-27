using System.Threading.Tasks;
using Application.Users;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/api/user-profile")]
    public class UserProfileController : BaseController
    {
        private readonly IGetUsersService _getUsersService;

        public UserProfileController(IGetUsersService getUsersService)
        {
            _getUsersService = getUsersService;
        }

        [HttpGet("{id}")]
        public async Task<UserProfile> GetUserProfile(string id)
        {
            var user = await _getUsersService.GetUserById(id);

            return new UserProfile(user);
        }
    }
}