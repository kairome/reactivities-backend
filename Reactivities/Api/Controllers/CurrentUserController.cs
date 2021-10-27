using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Users;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/api/current-user")]
    public class CurrentUserController : BaseController
    {
        private readonly ICurrentUserIdentity _currentUserIdentity;
        private readonly IWriteUsersService _writeUsersService;
        private readonly IPhotosService _photosService;

        public CurrentUserController(
            ICurrentUserIdentity currentUserIdentity,
            IWriteUsersService writeUsersService,
            IPhotosService photosService
        )
        {
            _currentUserIdentity = currentUserIdentity;
            _writeUsersService = writeUsersService;
            _photosService = photosService;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            return HandleResult(Result<UserDto>.Success(new UserDto(currentUser)));
        }

        [HttpPost("profile-photo")]
        public async Task<UserDto> AddPhoto([FromForm] IFormFile file)
        {
            var photoResult = await _photosService.AddPhoto(file);
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            var newUser = await _writeUsersService.AddProfilePhoto(currentUser.Id, new Photo(photoResult));
            return new UserDto(newUser);
        }
        
        [HttpDelete("photo/{id}")]
        public async Task<UserDto> AddPhoto(string id)
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();

            if (currentUser.ProfilePhoto?.Id == id)
            {
                throw new BadRequest("Profile photo cannot be deleted!");
            }

            if (currentUser.Photos.FirstOrDefault(x => x.Id == id) == null)
            {
                throw new BadRequest("Photo with this id does not exist!");
            }
            
            await _photosService.DeletePhoto(id);
            var newUser = await _writeUsersService.DeletePhoto(currentUser.Id, id);
            return new UserDto(newUser);
        }

        [HttpPut("profile-photo/{id}")]
        public async Task<UserDto> ChangeProfilePicture(string id)
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            if (currentUser.ProfilePhoto?.Id == id)
            {
                throw new BadRequest("Photo is already set as your profile picture");
            }

            var newProfilePhoto = currentUser.Photos.FirstOrDefault(x => x.Id == id);

            if (newProfilePhoto == null)
            {
                throw new BadRequest("Photo with this id is not found in your photos collection");
            }

            var updatedUser = await _writeUsersService.UpdateProfilePhoto(currentUser.Id, newProfilePhoto);
            return new UserDto(updatedUser);
        }

        [HttpPut]
        public async Task<UserDto> UpdateUser(UpdateUserDto dto)
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            var updatedUser = await _writeUsersService.UpdateUserInfo(currentUser.Id, dto);

            return new UserDto(updatedUser);
        }
    }
}