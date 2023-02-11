using Grpc.Net.Client;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Identity.API.Protos;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;

public class IdentityGrpcClient : IIdentityGrpcClient
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentityGrpcClient> _logger;

    public IdentityGrpcClient(IConfiguration configuration, ILogger<IdentityGrpcClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApplicationUser?> GetUserDetailsAsync(string userId)
    {
        var channel = GrpcChannel.ForAddress(_configuration["IdentityGrpcPlatform"]);
        var client = new GrpcIdentity.GrpcIdentityClient(channel);
        var request = new GetUserDetailsRequest()
        {
            UserId = userId
        };

        try
        {
            var response = await client.GetUserDetailsAsync(request);
            return new ApplicationUser
            {
                Id = response.GrpcUserDto.Id,
                FirstName = response.GrpcUserDto.FirstName,
                LastName = response.GrpcUserDto.LastName,
                Headline = response.GrpcUserDto.Headline,
                Biography = response.GrpcUserDto.Biography,
                PhotoPublicId = response.GrpcUserDto.PhotoPublicId,
                PhotoUrl = response.GrpcUserDto.PhotoUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not call the Catalog gRPC server: {ex.Message}");
            return null;
        }
    }
}
