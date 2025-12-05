using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModernBaseProject.Core.Behaviors;
using ModernBaseProject.Core.Constants;
using ModernBaseProject.Core.Interfaces;
using ModernBaseProject.Infrastructure.Authentication;
using ModernBaseProject.Infrastructure.Authorization;
using ModernBaseProject.Infrastructure.FileStorage;
using ModernBaseProject.Infrastructure.Notifications;
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
            options.UseNpgsql(configuration.GetConnectionString(ConfigurationKeys.DefaultConnection)));

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
                    ValidIssuer = configuration[ConfigurationKeys.Jwt.Issuer],
                    ValidAudience = configuration[ConfigurationKeys.Jwt.Audience],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[ConfigurationKeys.Jwt.Key]!))
                };

                // SignalR için token'ı query string'den al
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query[SignalRDefaults.AccessTokenQueryParameter];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(ApiRoutes.SignalR))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Permissions.UserCreate, policy => policy.Requirements.Add(new PermissionRequirement(Permissions.UserCreate)));
            options.AddPolicy(Permissions.UserRead, policy => policy.Requirements.Add(new PermissionRequirement(Permissions.UserRead)));
            options.AddPolicy(Permissions.UserUpdate, policy => policy.Requirements.Add(new PermissionRequirement(Permissions.UserUpdate)));
            options.AddPolicy(Permissions.UserDelete, policy => policy.Requirements.Add(new PermissionRequirement(Permissions.UserDelete)));
        });

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // Services
        services.AddScoped<JwtProvider>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<INotificationService, NotificationService>();

        // SignalR
        services.AddSignalR();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicies.AllowAll, builder =>
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
        app.UseCors(CorsPolicies.AllowAll);
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();
        app.MapHub<NotificationHub>(ApiRoutes.NotificationsHub);
        return app;
    }
}
