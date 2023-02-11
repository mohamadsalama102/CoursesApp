using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.EventBus.EventBus.Abstractions;
using nagiashraf.CoursesApp.EventBus.EventBus;
using nagiashraf.CoursesApp.Services.Catalog.API.Helpers.AutoMapper;
using nagiashraf.CoursesApp.Services.Catalog.Data;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;
using nagiashraf.CoursesApp.Services.Catalog.Services.CloudinaryFiles;
using nagiashraf.CoursesApp.Services.Catalog.Services.Courses;
using nagiashraf.CoursesApp.EventBus.EventBusRabbitMQ;
using Autofac;
using RabbitMQ.Client;
using nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;
using nagiashraf.CoursesApp.Services.Catalog.API.IntegrationEvents.EventHandling;
using nagiashraf.CoursesApp.Services.Catalog.API.Helpers;
using nagiashraf.CoursesApp.JwtAuthenticationManager;

namespace nagiashraf.CoursesApp.Services.Catalog.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment env)
    {
        var connectionString = string.Empty;
        if (env.IsDevelopment())
        {
            connectionString = configuration.GetConnectionString("CatalogConnection_Dev");
        }
        else
        {
            connectionString = configuration["CatalogConnectionString_Prod"];
        }

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddCustomJwtAuthentication(configuration);
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RequireModeratorRole", policy => policy.RequireRole("Admin", "Moderator"));
        });
        services.AddSwaggerGen();
        services.AddDbContext<CourseContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddAutoMapper(typeof(MappingProfiles));
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.Configure<CloudinarySettings>(configuration.GetSection(nameof(CloudinarySettings)));
        services.Configure<ApiUrl>(configuration.GetSection(nameof(ApiUrl)));
        services.AddGrpc();
        services.AddScoped<IIdentityGrpcClient, IdentityGrpcClient>();
        services.RegisterEventBus(configuration);
        services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQHost"],
                Port = int.Parse(configuration["RabbitMQPort"]),
                DispatchConsumersAsync = true
            };

            return new DefaultRabbitMQPersistentConnection(factory, logger);
        });


        return services;
    }

    public static IServiceCollection RegisterEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubscriptionsManager, subscriptionClientName);
        });

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        services.AddTransient<StudentEnrolledInCourseIntegrationEventHandler>();

        return services;
    }
}
