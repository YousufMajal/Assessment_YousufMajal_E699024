using Application.Behaviours;
using Application.Config;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

// Dependency injection configuration for the Application layer.
// Registers all application services including CQRS and behaviors.
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration options
        AddConfigurationOptions(services, configuration);

        // MediatR configuration for CQRS
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(AppAssemblyReference.assembly);
        });

        // Pipeline behaviors for cross-cutting concerns
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviourPipeline<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkPipelineBehavior<,>));

        return services;
    }

    private static void AddConfigurationOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BusinessRulesOptions>(configuration.GetSection(BusinessRulesOptions.SectionName));
    }
}