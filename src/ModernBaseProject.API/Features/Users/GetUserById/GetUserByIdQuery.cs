using MediatR;

namespace ModernBaseProject.API.Features.Users.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDetailDto>;
