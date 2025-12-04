using MediatR;
using Microsoft.EntityFrameworkCore;
using ModernBaseProject.Core.Domain.Entities;
using ModernBaseProject.Core.Exceptions;
using ModernBaseProject.Infrastructure.Authentication;
using ModernBaseProject.Infrastructure.Persistence;

namespace ModernBaseProject.API.Features.Users.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly AppDbContext _context;

    public CreateUserHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            throw new ValidationException("Email already exists");

        var roles = await _context.Roles
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            IsActive = true,
            Roles = roles
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateUserResponse(user.Id, user.Username, user.Email);
    }
}
