using MediatR;

namespace ModernBaseProject.API.Features.Users.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
