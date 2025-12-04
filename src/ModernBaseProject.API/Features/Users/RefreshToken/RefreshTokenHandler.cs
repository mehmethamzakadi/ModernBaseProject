using MediatR;
using Microsoft.EntityFrameworkCore;
using ModernBaseProject.API.Features.Users.Login;
using ModernBaseProject.Core.Exceptions;
using ModernBaseProject.Infrastructure.Authentication;
using ModernBaseProject.Infrastructure.Persistence;

namespace ModernBaseProject.API.Features.Users.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly AppDbContext _context;
    private readonly JwtProvider _jwtProvider;

    public RefreshTokenHandler(AppDbContext context, JwtProvider jwtProvider)
    {
        _context = context;
        _jwtProvider = jwtProvider;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedException("Invalid or expired refresh token");

        if (!user.IsActive)
            throw new UnauthorizedException("User is not active");

        var permissions = user.Roles
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission.Key)
            .Distinct()
            .ToList();

        var accessToken = _jwtProvider.GenerateAccessToken(user, permissions);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponse(accessToken, refreshToken, user.Email, user.Username);
    }
}
