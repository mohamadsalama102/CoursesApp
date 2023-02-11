using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.Services.Identity.API.Entities;
using nagiashraf.CoursesApp.Services.Identity.API.Protos;

namespace nagiashraf.CoursesApp.Services.Catalog.API.Grpc;

public class GrpcIdentityService : GrpcIdentity.GrpcIdentityBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GrpcIdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override async Task<IdentityResponse?> GetUserDetails(GetUserDetailsRequest request, ServerCallContext context)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == request.UserId);
        if (user == null) 
            return null;

        var grpcUserDto = new GrpcUserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Biography = user.Biography,
            Headline = user.Headline,
            PhotoPublicId = user.Photo?.PublicId,
            PhotoUrl = user.Photo?.Url
        };

        var response = new IdentityResponse
        {
            GrpcUserDto = grpcUserDto
        };

        return response;
    }
}
