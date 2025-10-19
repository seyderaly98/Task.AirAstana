namespace Task.AirAstana.Domain.Services;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}