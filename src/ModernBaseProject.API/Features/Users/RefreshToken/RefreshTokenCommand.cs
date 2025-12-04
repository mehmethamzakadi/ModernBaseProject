using MediatR;
using ModernBaseProject.API.Features.Users.Login;

namespace ModernBaseProject.API.Features.Users.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResponse>;
