using Application.Abstractions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Persistence.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? GetApplicationUserId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

        return claim is not null &&
               long.TryParse(claim.Value, out var id) ? id : null;
    }
}
