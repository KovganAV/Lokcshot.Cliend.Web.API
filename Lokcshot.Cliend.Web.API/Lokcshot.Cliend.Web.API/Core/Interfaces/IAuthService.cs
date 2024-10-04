using Lokcshot.Cliend.Web.API.Core.Classes;
using Discord.User.Models.User.Response;
using System.Security.Claims;

namespace Lokcshot.Cliend.Web.API.Core.Interfaces
{
    public interface IAuthService
    {
        ClaimsPrincipal ValidateToken(string token);

        AuthTokens CreateTokens(UserFullResponseModel userModel);

        string GenerateToken(UserFullResponseModel userModel, DateTime expires);
    }
}
