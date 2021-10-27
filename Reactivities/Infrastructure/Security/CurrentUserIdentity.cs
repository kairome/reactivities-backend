using System.Security.Claims;
using System.Threading.Tasks;
using Application.Core;
using Application.Users;
using Domain;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{

    public class CurrentUserIdentity : ICurrentUserIdentity
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGetUsersService _getUsersService;

        public CurrentUserIdentity(IHttpContextAccessor httpContextAccessor, IGetUsersService getUsersService)
        {
            _httpContextAccessor = httpContextAccessor;
            _getUsersService = getUsersService;
        }
        
        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<User> GetCurrentUser()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _getUsersService.GetUserById(userId);
        }
    }
}