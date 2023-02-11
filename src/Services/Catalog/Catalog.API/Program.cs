using Autofac.Extensions.DependencyInjection;
using nagiashraf.CoursesApp.Services.Catalog.API.Extensions;
using nagiashraf.CoursesApp.Services.Catalog.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

var app = builder.Build();

await app.MigrateDbContextAsync<CourseContext>(async (context, services) =>
{
    var env = services.GetRequiredService<IWebHostEnvironment>();
    var logger = services.GetRequiredService<ILogger<CourseContextSeed>>();

    await CourseContextSeed.SeedAsync(context, env);
});

// Configure the HTTP request pipeline.
app.ConfigureRequestPipeline();

await app.RunAsync();

public partial class Program { }