using Lokcshot.Cliend.Web.API.Core.Classes;
using Lokcshot.Cliend.Web.API.Core.Interfaces;
using Lokcshot.Cliend.Web.API.Settings;
using Discord.User.Models.User.Response;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lokcshot.Cliend.Web.API.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;

        public AuthService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.JwtSecret);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public AuthTokens CreateTokens(UserFullResponseModel userModel)
        {
            var accessToken = GenerateToken(userModel, DateTime.Now.AddMinutes(15));
            var refreshToken = GenerateToken(userModel, DateTime.Now.AddDays(7));

            return new AuthTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }

        public string GenerateToken(UserFullResponseModel userModel, DateTime expires) 
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
                new Claim(ClaimTypes.Name, userModel.Username),
                new Claim(ClaimTypes.Email, userModel.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.JwtSecret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                expires: expires
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
