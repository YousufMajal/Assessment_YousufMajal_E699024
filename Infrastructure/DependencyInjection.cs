using Amazon.SimpleNotificationService;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Aws;
using Infrastructure.Aws.Models;
using Infrastructure.Data;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

// Dependency injection configuration for  Infrastructure layer.
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration options
        AddConfigurationOptions(services, configuration);

        // Database configuration
        AddDatabase(services, configuration);

        // AWS services configuration
        AddAwsServices(services, configuration);

        // Repository registrations
        AddRepositories(services);

        // Background services
        AddBackgroundServices(services);

        return services;
    }

    private static void AddConfigurationOptions(IServiceCollection services, IConfiguration configuration)
    {
        // Infrastructure configuration options
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));
    }

    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        // Build DB connection string from env variables
        var connectionString = (configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing DefaultConnection in configuration."))
            .Replace("DB_HOST_SERVER", Environment.GetEnvironmentVariable("DB_HOST_SERVER") ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("DB_NAME", Environment.GetEnvironmentVariable("DB_NAME") ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("DB_USERNAME", Environment.GetEnvironmentVariable("DB_USERNAME") ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("DB_PASSWORD", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
    }

    private static void AddAwsServices(IServiceCollection services, IConfiguration configuration)
    {
        // AWS configuration options
        services.Configure<AwsOptions>(configuration.GetSection(AwsOptions.SectionName));

        // AWS SNS client
        services.AddSingleton<IAmazonSimpleNotificationService>(serviceProvider =>
        {
            var awsOptions = serviceProvider.GetRequiredService<IOptions<AwsOptions>>().Value;
            return new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.GetBySystemName(awsOptions.Region));
        });

        // AWS SNS service
        services.AddScoped<AwsSnsService>();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        // Repository pattern
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        // Unit of Work pattern
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Outbox services
        services.AddScoped<IOutboxWriter, OutboxWriter>();
    }

    private static void AddBackgroundServices(IServiceCollection services)
    {
        // Background services for outbox processing
        services.AddHostedService<OutboxBackgroundService>();
    }
}