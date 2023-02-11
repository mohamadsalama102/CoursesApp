using nagiashraf.CoursesApp.Services.Enrolling.API.Data.Repositories;
using nagiashraf.CoursesApp.Services.Enrolling.API.Data;
using nagiashraf.CoursesApp.Services.Enrolling.API.Services.Payments;
using Microsoft.EntityFrameworkCore;
using nagiashraf.CoursesApp.EventBus.EventBus.Abstractions;
using nagiashraf.CoursesApp.EventBus.EventBusRabbitMQ;
using nagiashraf.CoursesApp.Services.Enrolling.API.IntegrationEvents;
using RabbitMQ.Client;
using Autofac;
using nagiashraf.CoursesApp.EventBus.EventBus;
using nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;
using nagiashraf.CoursesApp.JwtAuthenticationManager;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env)
    {
        var connectionString = string.Empty;
        if (env.IsDevelopment())
        {
            connectionString = configuration.GetConnectionString("EnrollmentConnection_Dev");
        }
        else
        {
            connectionString = configuration["EnrollingConnectionString_Prod"];
        }

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddCustomJwtAuthentication(configuration);
        services.AddSwaggerGen();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowStripeTestingClient", policy =>
                policy.WithOrigins("http://127.0.0.1:5500")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
        services.AddDbContext<EnrollmentContext>(options =>
                    options.UseSqlServer(connectionString));
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.Configure<StripeSettings>(configuration.GetSection(nameof(StripeSettings)));
        services.AddScoped<ICatalogGrpcClient, CatalogGrpcClient>();
        services.AddIntegrationServices(configuration);
        services.AddEventBus(configuration);

        return services;
    }

    public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEnrollingIntegrationEventService, EnrollingIntegrationEventService>();

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

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName);
        });
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        return services;
    }
}
