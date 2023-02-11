using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;

public interface IIdentityGrpcClient
{
    Task<ApplicationUser?> GetUserDetailsAsync(string userId);
}
