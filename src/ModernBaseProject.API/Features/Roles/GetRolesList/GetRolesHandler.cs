using MediatR;
using Microsoft.EntityFrameworkCore;
using ModernBaseProject.Infrastructure.Persistence;

namespace ModernBaseProject.API.Features.Roles.GetRolesList;

public class GetRolesHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly AppDbContext _context;

    public GetRolesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Roles
            .Where(r => !r.IsDeleted)
            .Select(r => new RoleDto(r.Id, r.Name))
            .ToListAsync(cancellationToken);
    }
}