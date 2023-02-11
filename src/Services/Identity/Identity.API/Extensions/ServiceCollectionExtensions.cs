using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.JwtAuthenticationManager;
using nagiashraf.CoursesApp.Services.Identity.API.Data;
using nagiashraf.CoursesApp.Services.Identity.API.Entities;
using nagiashraf.CoursesApp.Services.Identity.API.Helpers;
using nagiashraf.CoursesApp.Services.Identity.API.Services.Emails;
using nagiashraf.CoursesApp.Services.Identity.API.Services.Tokens;

namespace nagiashraf.CoursesApp.Services.Identity.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env)
    {
        var connectionString = string.Empty;
        if (env.IsDevelopment())
        {
            connectionString = configuration.GetConnectionString("IdentityConnection_Dev");
        }
        else
        {
            connectionString = configuration["IdentityConnectionString_Prod"];
        }

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.SignIn.RequireConfirmedEmail = true;
        }).AddRoles<ApplicationRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();
        services.AddCustomJwtAuthentication(configuration);
        services.AddAuthorization(options => 
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RequireModeratorRole", policy => policy.RequireRole("Admin", "Moderator"));
        });
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
        services.Configure<ApiUrl>(configuration.GetSection(nameof(ApiUrl)));
        services.AddGrpc();

        return services;
    }
}