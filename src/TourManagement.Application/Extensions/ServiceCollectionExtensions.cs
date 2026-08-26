using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Extensions;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Application layer services with the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(MappingProfile).Assembly);

        // Register application services (both as interface and concrete for direct injection)
        services.AddScoped<UserService>();
        services.AddScoped<TourService>();
        services.AddScoped<BookingService>();

        // Register as interfaces for domain-level usage
        services.AddScoped<IUserService>(sp => sp.GetRequiredService<UserService>());
        services.AddScoped<ITourService>(sp => sp.GetRequiredService<TourService>());
        services.AddScoped<IBookingService>(sp => sp.GetRequiredService<BookingService>());

        return services;
    }
}
