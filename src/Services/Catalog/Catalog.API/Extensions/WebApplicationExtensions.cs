using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.EventBus.EventBus.Abstractions;
using nagiashraf.CoursesApp.Services.Catalog.API.Grpc;
using nagiashraf.CoursesApp.Services.Catalog.API.IntegrationEvents.EventHandling;
using nagiashraf.CoursesApp.Services.Catalog.API.IntegrationEvents.Events;

namespace nagiashraf.CoursesApp.Services.Catalog.API.Extensions;

public static class WebApplicationExtensions
{
    public async static Task<WebApplication> MigrateDbContextAsync<TContext>(this WebApplication app,
        Func<TContext, IServiceProvider, Task> seeder)
        where TContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

            await context.Database.MigrateAsync();

            await seeder(context, services);

            logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            if (app.Environment.IsDevelopment())
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
            }
            else
            {
                logger.LogError("Internal Server Error");
            }
        }
        return app;
    }

    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapGrpcService<GrpcCatalogService>();

        app.MapGet("/protos/catalog.proto", async context =>
        {
            await context.Response.WriteAsync(File.ReadAllText("../Protos/catalog.proto"));
        });

        app.ConfigureEventBus();

        return app;
    }

    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

        eventBus.Subscribe<StudentEnrolledInCourseIntegrationEvent, StudentEnrolledInCourseIntegrationEventHandler>();
    }
}
