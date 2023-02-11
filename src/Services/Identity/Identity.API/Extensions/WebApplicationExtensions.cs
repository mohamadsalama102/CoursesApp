using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.Services.Catalog.API.Grpc;
using nagiashraf.CoursesApp.Services.Identity.API.Data;
using nagiashraf.CoursesApp.Services.Identity.API.Entities;

namespace nagiashraf.CoursesApp.Services.Identity.API.Extensions;

public static class WebApplicationExtensions
{
    public async static Task<WebApplication> MigrateIdentityDbContextAsync(this WebApplication app,
        Func<UserManager<ApplicationUser>, RoleManager<ApplicationRole>, IServiceProvider, Task> seeder)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILogger<ApplicationUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        try
        {
            logger.LogInformation("Migrating Identity database");

            await context.Database.MigrateAsync();

            await seeder(userManager, roleManager, services);

            logger.LogInformation("Migrated Identity database");
        }
        catch (Exception ex)
        {
            if (app.Environment.IsDevelopment())
            {
                logger.LogError(ex, "An error occurred while migrating Identity database");
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

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapGrpcService<GrpcIdentityService>();

        app.MapGet("/protos/identity.proto", async context =>
        {
            await context.Response.WriteAsync(File.ReadAllText("../Protos/identity.proto"));
        });

        return app;
    }
}
