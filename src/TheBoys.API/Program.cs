using System.Reflection;
using System.Threading.RateLimiting;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using TheBoys.Application;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Infrastructure;

namespace TheBoys.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{builder.Environment.EnvironmentName}.Local.json",
                optional: true,
                reloadOnChange: true
            );

        var allowedOrigins = GetAllowedOrigins(builder.Configuration);
        var uploadsOptions =
            builder.Configuration.GetSection("Uploads").Get<UploadsOptions>()
            ?? new UploadsOptions();
        var uploadRoot = ResolveUploadsRoot(
            builder.Environment.ContentRootPath,
            uploadsOptions.RootPath
        );

        if (
            !Directory.Exists(uploadRoot)
            && !Path.IsPathRooted(uploadsOptions.RootPath ?? string.Empty)
        )
        {
            Directory.CreateDirectory(uploadRoot);
        }

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc(
                "the.boys.api",
                new OpenApiInfo { Title = "the.boys.api", Version = "v1" }
            );
            s.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description =
                        "If you hit endpoints from swagger, enter token directly | If you hit endpoints from client side app, enter 'Bearer [token]'"
                }
            );
            s.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            );
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddCors(cors =>
        {
            cors.AddPolicy(
                "the.boys.policy",
                options =>
                    options
                        .SetIsOriginAllowed(origin =>
                            IsExplicitlyAllowedOrigin(origin, allowedOrigins)
                            || IsLocalDevelopmentOrigin(origin)
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetPreflightMaxAge(TimeSpan.FromMinutes(30))
            );
        });

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        $"IP_{httpContext.Connection.RemoteIpAddress}",
                        _ =>
                            new FixedWindowRateLimiterOptions
                            {
                                Window = TimeSpan.FromMinutes(1),
                                PermitLimit = 50
                            }
                    )
            );
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var seedingService = scope.ServiceProvider.GetRequiredService<ISeedingService>();
            seedingService.SeedLanguages();
        }

        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        if (Directory.Exists(uploadRoot))
        {
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(uploadRoot),
                    RequestPath = NormalizeRequestPath(uploadsOptions.RequestPath)
                }
            );
        }

        app.UseRouting();
        app.UseCors("the.boys.policy");
        app.UseAuthorization();
        app.UseRateLimiter();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/the.boys.api/swagger.json", "the.boys.api");
            c.DisplayRequestDuration();
            c.EnableFilter();
            c.EnablePersistAuthorization();
        });

        app.MapControllers();
        app.MapFallbackToFile("index.html");
        app.Run();
    }

    private static string[] GetAllowedOrigins(IConfiguration configuration)
    {
        var configuredOrigins =
            configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        var defaultOrigins = new[]
        {
            "http://localhost:5173",
            "http://193.227.24.31:5000",
            "http://stage.menofia.edu.eg:5000",
            "https://stage.menofia.edu.eg:5000",
            "http://stage.menofia.edu.eg:5050",
            "https://stage.menofia.edu.eg:5050",
            "http://stage.menofia.edu.eg",
            "https://stage.menofia.edu.eg",
            "http://mu.menofia.edu.eg",
            "https://mu.menofia.edu.eg"
        };

        return configuredOrigins
            .Concat(defaultOrigins)
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim().TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool IsExplicitlyAllowedOrigin(string origin, string[] allowedOrigins)
    {
        if (string.IsNullOrWhiteSpace(origin))
        {
            return false;
        }

        var normalizedOrigin = origin.Trim().TrimEnd('/');
        return allowedOrigins.Contains(normalizedOrigin, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsLocalDevelopmentOrigin(string origin)
    {
        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var isHttpScheme =
            uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            || uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);

        if (!isHttpScheme)
        {
            return false;
        }

        return uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
            || uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveUploadsRoot(string contentRootPath, string configuredRootPath)
    {
        if (!string.IsNullOrWhiteSpace(configuredRootPath))
        {
            return Path.GetFullPath(
                Path.IsPathRooted(configuredRootPath)
                    ? configuredRootPath
                    : Path.Combine(contentRootPath, configuredRootPath)
            );
        }

        return Path.GetFullPath(Path.Combine(contentRootPath, "uploads"));
    }

    private static PathString NormalizeRequestPath(string requestPath)
    {
        if (string.IsNullOrWhiteSpace(requestPath))
        {
            return new PathString("/uploads");
        }

        return requestPath.StartsWith('/')
            ? new PathString(requestPath)
            : new PathString($"/{requestPath}");
    }

 
}
