using MediatR;
using Microsoft.EntityFrameworkCore;
using ModernBaseProject.Core.Exceptions;
using ModernBaseProject.Infrastructure.Persistence;

namespace ModernBaseProject.API.Features.Users.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDetailDto>
{
    private readonly AppDbContext _context;

    public GetUserByIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDetailDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .Where(u => u.Id == request.Id)
            .Select(u => new UserDetailDto(
                u.Id,
                u.Username,
                u.Email,
                u.IsActive,
                u.Roles.Select(r => new RoleDto(r.Id, r.Name)).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            throw new NotFoundException("User not found");

        return user;
    }
}

public record UserDetailDto(Guid Id, string Username, string Email, bool IsActive, List<RoleDto> Roles);
public record RoleDto(Guid Id, string Name);
