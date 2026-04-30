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
        var connectionString = configuration.GetConnectionString(
            isDevelopment ? "LocalDatabaseConnection" : "ProductionDatabaseConnection"
        );
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = configuration.GetConnectionString("LocalDatabaseConnection");
        }
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string is not configured. Add LocalDatabaseConnection or ProductionDatabaseConnection."
            );
        }

        services.AddDbContext<ApplicationDbContext>(cfg =>
            cfg.UseSqlServer(connectionString)
        );

        services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
        services.Configure<FileUrlOptions>(configuration.GetSection(FileUrlOptions.SectionName));
        services.PostConfigure<FileUrlOptions>(options =>
        {
            options.UploadsRequestPath =
                configuration["Uploads:RequestPath"] ?? options.UploadsRequestPath;
        });
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailSettings>>().Value);

        services.AddScoped<IFileUrlService, FileUrlService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<INewsStatisticsService, NewsStatisticsService>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<ISeedingService, SeedingService>();

        return services;
    }
}
