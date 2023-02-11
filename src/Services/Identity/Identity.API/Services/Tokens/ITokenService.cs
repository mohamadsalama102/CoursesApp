using nagiashraf.CoursesApp.Services.Identity.API.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace nagiashraf.CoursesApp.Services.Identity.API.Services.Tokens;

public interface ITokenService
{
    Task<JwtSecurityToken> CreateTokenAsync(ApplicationUser user);
    string CreateRefreshToken();

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
