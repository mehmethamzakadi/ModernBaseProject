using MediatR;
using Microsoft.EntityFrameworkCore;
using ModernBaseProject.Core.Exceptions;
using ModernBaseProject.Infrastructure.Persistence;

namespace ModernBaseProject.API.Features.Users.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly AppDbContext _context;

    public DeleteUserHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);

        if (user == null)
            throw new NotFoundException("User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
