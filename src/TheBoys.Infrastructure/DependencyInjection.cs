using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Settings;
using TheBoys.Infrastructure.Persistence;
using TheBoys.Infrastructure.Services;

namespace TheBoys.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment
    )
    {
        services.AddDbContext<ApplicationDbContext>(cfg =>
            cfg.UseSqlServer(
                configuration.GetConnectionString(
                    isDevelopment ? "LocalDatabaseConnection" : "ProductionDatabaseConnection"
                )
            )
        );

        services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailSettings>>().Value);

        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<ISeedingService, SeedingService>();

        return services;
    }
}
