using MediatR;

namespace ModernBaseProject.API.Features.Users.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string Username,
    string Email,
    bool IsActive,
    List<Guid> RoleIds
) : IRequest<UpdateUserResponse>;
