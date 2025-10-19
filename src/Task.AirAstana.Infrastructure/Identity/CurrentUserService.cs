using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Task.AirAstana.Domain.Services;

namespace Task.AirAstana.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) 
                             ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) 
                               ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("name");

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }
}