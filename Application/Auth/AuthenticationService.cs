using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Users;
using AspNetCore.ServiceRegistration.Dynamic;
using Domain;

namespace Application.Auth
{
    [ScopedService]
    public interface IAuthenticationService
    {
        Task<(UserDto, string)> Login(LoginDto dto);
        Task<UserDto> Register(NewUserDto dto);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IGetUsersService _getUsersService;
        private readonly IWriteUsersService _writeUsersService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(
            IGetUsersService getUsersService,
            IWriteUsersService writeUsersService,
            ITokenService tokenService
        )
        {
            _getUsersService = getUsersService;
            _writeUsersService = writeUsersService;
            _tokenService = tokenService;
        }

        public async Task<(UserDto, string)> Login(LoginDto dto)
        {
            var user = (await _getUsersService.GetUsersByNameOrEmail(dto.Login, dto.Login)).FirstOrDefault();
            if (user == null || !PasswordService.VerifyPassword(dto.Password, user.Hash, user.Salt))
            {
                throw new NotAuthorized("Login credentials are invalid");
            }

            var jwtToken = _tokenService.CreateToken(user);

            return (new UserDto(user), jwtToken);
        }

        public async Task<UserDto> Register(NewUserDto dto)
        {
            var existingUsers = await _getUsersService.GetUsersByNameOrEmail(dto.UserName, dto.Email);

            if (existingUsers.Any())
            {
                throw new BadRequest("A user with this name or email already exists");
            }

            var newUser = await _writeUsersService.CreateUser(dto);
            return new UserDto(newUser);
        }
    }
}