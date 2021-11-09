using System.Threading.Tasks;
using Application.Auth;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/api/account")]
    public class AccountController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(
            IAuthenticationService authenticationService
        )
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<UserDto> Login(LoginDto loginDto)
        {
            var (userDto, token) = await _authenticationService.Login(loginDto);

            var options = new CookieOptions();
            options.Expires = AuthConfigs.TokenExpiryDate;
            options.Secure = false;
            options.HttpOnly = true;
            
            options.SameSite = SameSiteMode.Strict;
            Response.Cookies.Append(AuthConfigs.AuthCookieName, token, options);
            return userDto;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<UserDto> Register(NewUserDto newUser)
        {
            return await _authenticationService.Register(newUser);
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            
            Response.Cookies.Delete(AuthConfigs.AuthCookieName);
            return Ok();
        }
    }
}