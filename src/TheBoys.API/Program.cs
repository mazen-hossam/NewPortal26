using System.Reflection;
using System.Threading.RateLimiting;
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
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(origin =>
                            origin.StartsWith("http://localhost:5173")
                            || origin == "http://193.227.24.31:5000"
                            || origin == "http://stage.menofia.edu.eg:5000"
                            || origin == "https://stage.menofia.edu.eg:5000"
                            || origin == "https://stage.menofia.edu.eg"
                            || origin == "http://stage.menofia.edu.eg"
                        )
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

        app.UseCors("the.boys.policy");
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/the.boys.api/swagger.json", "the.boys.api");
            c.DisplayRequestDuration();
            c.EnableFilter();
            c.EnablePersistAuthorization();
        });

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapFallbackToFile("index.html");
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
