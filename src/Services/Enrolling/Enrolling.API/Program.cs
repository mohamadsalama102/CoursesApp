using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.Services.Enrolling.API.Data;
using nagiashraf.CoursesApp.Services.Enrolling.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var services = serviceScope.ServiceProvider;
var context = services.GetRequiredService<EnrollmentContext>();
await context.Database.MigrateAsync();

// Configure the HTTP request pipeline.
app.ConfigureRequestPipeline();

app.Run();
