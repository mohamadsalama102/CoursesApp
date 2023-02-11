using nagiashraf.CoursesApp.ApiGateways.Web.Bff.Middleware;

namespace nagiashraf.CoursesApp.ApiGateways.Web.Bff.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
