using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.Services.Identity.API.Entities;
using System.Net;

namespace nagiashraf.CoursesApp.Services.Identity.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Policy = "RequireAdminRole")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("users")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .OrderBy(user => user.FirstName)
            .Select(user => new {
                Id = user.Id,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                Roles = user.UserRoles.Select(r => r.Role.Name)
            })
            .ToListAsync();
        return Ok(users);
    }

    [HttpPost("edit-roles/{userId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> EditRoles(string userId, [FromQuery] string roles)
    {
        var selectedRoles = roles.Split(',');

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found");

        var currentRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(currentRoles));
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        result = await _userManager.RemoveFromRolesAsync(user, currentRoles.Except(selectedRoles));
        if (!result.Succeeded) 
            return BadRequest(result.Errors);

        return Ok(await _userManager.GetRolesAsync(user));
    }
}
