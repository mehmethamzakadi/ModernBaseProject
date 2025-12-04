using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModernBaseProject.Core.Behaviors;
using ModernBaseProject.Core.Interfaces;
using ModernBaseProject.Infrastructure.Authentication;
using ModernBaseProject.Infrastructure.Authorization;
using ModernBaseProject.Infrastructure.FileStorage;
using ModernBaseProject.Infrastructure.Notifications.Hubs;
using ModernBaseProject.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;

namespace ModernBaseProject.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        // Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
                
                // SignalR için token'ı query string'den al
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("User.Create", policy => policy.Requirements.Add(new PermissionRequirement("User.Create")));
            options.AddPolicy("User.Read", policy => policy.Requirements.Add(new PermissionRequirement("User.Read")));
            options.AddPolicy("User.Update", policy => policy.Requirements.Add(new PermissionRequirement("User.Update")));
            options.AddPolicy("User.Delete", policy => policy.Requirements.Add(new PermissionRequirement("User.Delete")));
        });

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // Services
        services.AddScoped<JwtProvider>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // SignalR
        services.AddSignalR();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.WithOrigins("http://localhost:3000")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        return services;
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        app.UseMiddleware<API.Middleware.GlobalExceptionHandler>();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();
        app.MapHub<NotificationHub>("/hubs/notifications");
        return app;
    }
}
