using MediatR;
using Microsoft.EntityFrameworkCore;
using ModernBaseProject.Core.Exceptions;
using ModernBaseProject.Infrastructure.Notifications;
using ModernBaseProject.Infrastructure.Persistence;

namespace ModernBaseProject.API.Features.Users.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public DeleteUserHandler(AppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);

        if (user == null)
            throw new NotFoundException("User not found");

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        
        await _notificationService.SendToAllAsync($"User '{user.Username}' has been deleted.");
    }
}
