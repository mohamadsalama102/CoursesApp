using Grpc.Core;
using nagiashraf.CoursesApp.Services.Catalog.API.Protos;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;

namespace nagiashraf.CoursesApp.Services.Catalog.API.Grpc;

public class GrpcCatalogService : GrpcCatalog.GrpcCatalogBase
{
    private readonly ICourseRepository _courseRepository;

    public GrpcCatalogService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public override async Task<CatalogResponse> GetCoursePrice(GetCoursePriceRequest request, ServerCallContext context)
    {
        var response = new CatalogResponse
        {
            Price = await _courseRepository.GetCoursePriceAsync(request.CourseId)
        };

        return response;
    }
}
