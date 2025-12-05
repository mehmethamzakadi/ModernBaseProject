using MediatR;
using Microsoft.EntityFrameworkCore;
using ModernBaseProject.Core.Domain.Entities;
using ModernBaseProject.Core.Exceptions;
using ModernBaseProject.Infrastructure.Authentication;
using ModernBaseProject.Infrastructure.Notifications;
using ModernBaseProject.Infrastructure.Persistence;

namespace ModernBaseProject.API.Features.Users.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public CreateUserHandler(AppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (existingUser != null)
        {
            if (!existingUser.IsDeleted)
                throw new ValidationException("This email is already in use by an active user.");

            existingUser.Username = request.Username;
            existingUser.PasswordHash = PasswordHasher.HashPassword(request.Password);
            existingUser.IsActive = true;
            existingUser.IsDeleted = false;
            existingUser.DeletedAt = null;
            existingUser.DeletedBy = null;
            existingUser.UpdatedAt = DateTime.UtcNow;

            existingUser.Roles.Clear();
            if (request.RoleIds.Any())
            {
                var roles = await _context.Roles
                    .Where(r => request.RoleIds.Contains(r.Id))
                    .ToListAsync(cancellationToken);
                
                foreach (var role in roles)
                {
                    existingUser.Roles.Add(role);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            await _notificationService.SendToAllAsync($"User '{existingUser.Username}' has been restored.");

            return new CreateUserResponse(existingUser.Id, existingUser.Username, existingUser.Email);
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        if (request.RoleIds.Any())
        {
            var roles = await _context.Roles
                .Where(r => request.RoleIds.Contains(r.Id))
                .ToListAsync(cancellationToken);
            
            foreach (var role in roles)
            {
                user.Roles.Add(role);
            }
            
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        await _notificationService.SendToAllAsync($"New user '{user.Username}' has been created.");

        return new CreateUserResponse(user.Id, user.Username, user.Email);
    }
}
