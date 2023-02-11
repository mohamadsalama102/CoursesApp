using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using nagiashraf.CoursesApp.JwtAuthenticationManager;
using nagiashraf.CoursesApp.Services.Identity.API.DTOs;
using nagiashraf.CoursesApp.Services.Identity.API.Entities;
using nagiashraf.CoursesApp.Services.Identity.API.Services.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace nagiashraf.CoursesApp.Services.Identity.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public TokenController(ITokenService tokenService, UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UserDto>> RefreshToken(TokenDto tokenDto)
    {
        if (tokenDto == null || tokenDto.AccessToken == null || tokenDto.RefreshToken == null)
            return BadRequest("Invalid token");

        var principal = _tokenService.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        if (principal == null) 
            return BadRequest("Invalid token");

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || user.RefreshToken.Token != tokenDto.RefreshToken || user.RefreshToken.ExpirationDate <= DateTime.UtcNow)
            return BadRequest("Invalid token");

        var newRefreshToken = _tokenService.CreateRefreshToken();
        var newAccessSecurityToken = await _tokenService.CreateTokenAsync(user);

        user.RefreshToken.Token = newRefreshToken;
        user.RefreshToken.ExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
        await _userManager.UpdateAsync(user);

        return new UserDto
        {
            UserId = user.Id,
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessSecurityToken),
            AccessTokenExpirationTime = newAccessSecurityToken.ValidTo,
            RefreshToken = newRefreshToken,
            RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays)
        };
    }
}
