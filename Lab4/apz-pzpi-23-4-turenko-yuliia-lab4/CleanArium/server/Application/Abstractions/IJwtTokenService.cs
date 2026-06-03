using Domain.Models;

namespace Application.Abstractions;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    RefreshToken GenerateRefreshToken();
}
