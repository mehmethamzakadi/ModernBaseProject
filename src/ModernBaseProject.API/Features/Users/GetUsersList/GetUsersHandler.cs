using MediatR;
using Microsoft.EntityFrameworkCore;
using ModernBaseProject.Infrastructure.Persistence;

namespace ModernBaseProject.API.Features.Users.GetUsersList;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly AppDbContext _context;

    public GetUsersHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .Select(u => new UserDto(
                u.Id, 
                u.Username, 
                u.Email, 
                u.IsActive,
                u.Roles.Select(r => new RoleDto(r.Id, r.Name)).ToList()
            ))
            .ToListAsync(cancellationToken);
    }
}
