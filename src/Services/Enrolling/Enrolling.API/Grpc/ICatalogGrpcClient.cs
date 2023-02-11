namespace nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;

public interface ICatalogGrpcClient
{
    Task<decimal?> GetCoursePriceAsync(int courseId);
}
