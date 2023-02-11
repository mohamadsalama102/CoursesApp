using Grpc.Net.Client;
using nagiashraf.CoursesApp.Services.Catalog.API.Protos;
using nagiashraf.CoursesApp.Services.Enrolling.API.Grpc.Extensions;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;

public class CatalogGrpcClient : ICatalogGrpcClient
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CatalogGrpcClient> _logger;

    public CatalogGrpcClient(IConfiguration configuration, ILogger<CatalogGrpcClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<decimal?> GetCoursePriceAsync(int courseId)
    {
        var channel = GrpcChannel.ForAddress(_configuration["CatalogGrpcPlatform"]);
        var client = new GrpcCatalog.GrpcCatalogClient(channel);
        var request = new GetCoursePriceRequest()
        {
            CourseId = courseId
        };

        try
        {
            var response = await client.GetCoursePriceAsync(request);
            return response.Price.ToDecimal();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Could not call the Catalog gRPC server: {ex.Message}");
            return null;
        }
    }
}
