using nagiashraf.CoursesApp.Services.Identity.API.Data;
using nagiashraf.CoursesApp.Services.Identity.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

var app = builder.Build();

await app.MigrateIdentityDbContextAsync(async (userManager, roleManager, services) =>
{
    var env = services.GetRequiredService<IWebHostEnvironment>();
    var logger = services.GetRequiredService<ILogger<AppIdentityDbContextSeed>>();

    await AppIdentityDbContextSeed.SeedRolesAsync(roleManager, env);
    await AppIdentityDbContextSeed.SeedUsersAsync(userManager, env);
});

// Configure the HTTP request pipeline.
app.ConfigureRequestPipeline();

app.Run();
