using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using nagiashraf.CoursesApp.JwtAuthenticationManager;
using nagiashraf.CoursesApp.Services.Identity.API.DTOs;
using nagiashraf.CoursesApp.Services.Identity.API.Entities;
using nagiashraf.CoursesApp.Services.Identity.API.Helpers;
using nagiashraf.CoursesApp.Services.Identity.API.Services.Emails;
using nagiashraf.CoursesApp.Services.Identity.API.Services.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace nagiashraf.CoursesApp.Services.Identity.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ApiUrl _apiUrl;
        private readonly JwtSettings _jwtSettings;

        public AccountController(UserManager<ApplicationUser> userManager, ITokenService tokenService, IEmailService emailService,
            IOptions<JwtSettings> jwtSettings, IOptions<ApiUrl> apiUrl)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _apiUrl = apiUrl.Value;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await EmailExistsAsync(registerDto.Email)) 
                return BadRequest("There was a problem creating your account. Check that your email address is spelled correctly.");

            var user = new ApplicationUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            SetUserRefreshToken(user);

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded) 
                return BadRequest(roleResult.Errors);

            await SendEmailConfirmationLink(user);

            return Ok("Registration successful. An email was sent to confirm your email address.");
        }

        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshToken)
                .SingleOrDefaultAsync(x => x.NormalizedEmail == loginDto.Email.ToUpper());

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Unauthorized("Username or password not valid");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized("Email not verified yet");

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
                if (!providers.Contains("Email"))
                    return BadRequest("Only Email provider is available");

                await SendEmailTwoFactorAuthToken(user);

                return Ok($"An email containing the two-factor authentication token was sent to {loginDto.Email}. Please Add in the token.");
            }

            var jwtSecurityToken = await _tokenService.CreateTokenAsync(user);

            SetUserRefreshToken(user);

            await _userManager.UpdateAsync(user);

            return new UserDto
            {
                UserId = user.Id,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                AccessTokenExpirationTime = jwtSecurityToken.ValidTo,
                RefreshToken = user.RefreshToken.Token,
                RefreshTokenExpirationTime = user.RefreshToken.ExpirationDate
            };
        }

        [HttpPost("login-2f")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserDto>> LoginTwoFactorAuth(LoginTwoFactorDto loginTwoFactorDto)
        {
            var user = await _userManager.FindByEmailAsync(loginTwoFactorDto.Email);
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", loginTwoFactorDto.TwoFactorAuthCode);
            if (!result)
                return BadRequest("Invalid two-factor authentication code.");

            var jwtSecurityToken = await _tokenService.CreateTokenAsync(user);

            SetUserRefreshToken(user);

            await _userManager.UpdateAsync(user);

            return new UserDto
            {
                UserId = user.Id,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                AccessTokenExpirationTime = jwtSecurityToken.ValidTo,
                RefreshToken = user.RefreshToken.Token,
                RefreshTokenExpirationTime = user.RefreshToken.ExpirationDate
            };
        }

        private void SetUserRefreshToken(ApplicationUser user)
        {
            var newRefreshToken = _tokenService.CreateRefreshToken();
            var newRefreshTokenExpirationTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);

            user.RefreshToken = new RefreshToken
            {
                Token = newRefreshToken,
                ExpirationDate = newRefreshTokenExpirationTime
            };
        }

        private async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        private async Task SendEmailConfirmationLink(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string?>
            {
                { "email", user.Email },
                { "token", token }
            };
            var confirmationLink = QueryHelpers.AddQueryString(_apiUrl.BaseUrl + "/account/confirm-email", param);
            var emailBody = $"Dear {user.FirstName},\n\nWe are so glad you decided to join us. Kindly click here to confirm your " +
                $"email:\n{confirmationLink}\n\nPlease ignore this email if you did not register with us\n\nRegards,\nThe CoursesApp team";
            var emailRequest = new EmailRequest(new string[] { user.Email }, "Email Confirmation", emailBody);
            await _emailService.SendEmailAsync(emailRequest);
        }

        private async Task SendEmailTwoFactorAuthToken(ApplicationUser user)
        {
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var emailBody = $"Dear {user.FirstName},\n\nHere is the code required to complete your two-step login: {token}\n\nThe " +
                $"CoursesApp team";
            var emailRequest = new EmailRequest(new string[] { user.Email }, "Two-Factor Authentication code", emailBody);
            await _emailService.SendEmailAsync(emailRequest);
        }

        [HttpGet("confirm-email")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (email == null || token == null)
                return BadRequest("Invalid Email Confirmation Request");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return BadRequest(result.Errors);
            
            return NoContent();
        }

        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }

        [Authorize]
        [HttpPost("enable-2f-auth")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }

        [Authorize]
        [HttpPost("disable-2f-auth")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
    }
}
