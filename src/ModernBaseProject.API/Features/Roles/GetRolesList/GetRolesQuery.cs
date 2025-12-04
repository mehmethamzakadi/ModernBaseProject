using MediatR;

namespace ModernBaseProject.API.Features.Roles.GetRolesList;

public record GetRolesQuery : IRequest<List<RoleDto>>;

public record RoleDto(Guid Id, string Name);