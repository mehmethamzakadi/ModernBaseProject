using ModernBaseProject.API.Extensions;
using ModernBaseProject.API.Features.Files.UploadFile;
using ModernBaseProject.API.Features.Users.CreateUser;
using ModernBaseProject.API.Features.Users.DeleteUser;
using ModernBaseProject.API.Features.Users.GetUserById;
using ModernBaseProject.API.Features.Users.GetUsersList;
using ModernBaseProject.API.Features.Users.Login;
using ModernBaseProject.API.Features.Users.RefreshToken;
using ModernBaseProject.API.Features.Users.UpdateUser;
using ModernBaseProject.API.Features.Roles.GetRolesList;
using ModernBaseProject.Infrastructure.Persistence;
using ModernBaseProject.Infrastructure.Persistence.Seeding;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration["Seq:Url"] ?? "http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseSeeder.SeedAsync(context);
}

// Configure
app.ConfigureApplication();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map Endpoints
app.MapLoginEndpoint();
app.MapRefreshTokenEndpoint();
app.MapGetUsersEndpoint();
app.MapGetUserByIdEndpoint();
app.MapCreateUserEndpoint();
app.MapUpdateUserEndpoint();
app.MapDeleteUserEndpoint();
app.MapUploadFileEndpoint();
app.MapGetRolesEndpoint();

app.MapGet("/", () => "Modern Base Project API is running!");

app.Run();
