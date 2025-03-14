using Lokcshot.Cliend.Web.API.Core.Classes;
using Lokcshot.Cliend.Web.API.Core.Interfaces;
using Lokcshot.Cliend.Web.API.Core.Methods;
using Lockshot.User.API.Class;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Net;
using System.Security.Claims;

namespace Lokcshot.Cliend.Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRefit _userRefit;
        private readonly IAuthService _authService;

        public AuthController(IUserRefit userRefit, IAuthService authService)
        {
            _userRefit = userRefit;
            _authService = authService;
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("getAllUsers")]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            var users = await _userRefit.GetAllUsers();
            return Ok(users);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("getUser/{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            try
            {
                var user = await _userRefit.GetUserFullResponseModelById(id);
                return Ok(user);
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound) 
            {
                return BadRequest("User not found.");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthTokens>> Login(LoginUserDto loginModel)
        {
            try
            {
                var user = await _userRefit.Login(loginModel);
                var tokens = _authService.CreateTokens(user);

                return Ok(tokens);
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                return BadRequest("Invalid email or password.");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterUserDto createModel)
        {
            try
            {
                var user = await _userRefit.Register(createModel);
                var tokens = _authService.CreateTokens(user);

                return Ok(tokens);
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                var validationError = ParseValidation.ParseValidationError(ex.Content);
                return BadRequest(validationError);
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return BadRequest("Email is already exist");
            }
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken([FromBody] string refreshToken)
        {
            var principal = _authService.ValidateToken(refreshToken);
            if (principal == null)
                return BadRequest("1Invalid refresh token.");

            var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (userId == null)
                return BadRequest("Invalid refresh token.");

            Guid userGuid;
            if (Guid.TryParse(userId, out userGuid))
            {
                var user = await _userRefit.GetUserFullResponseModelById(userGuid);
                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                var accessToken = _authService.GenerateToken(user, DateTime.Now.AddMinutes(15));
                return Ok(new { AccessToken = accessToken });
            }
            else
                return BadRequest($"Unable to parse guid in refresh token: {userId}");

        }

    }
}
