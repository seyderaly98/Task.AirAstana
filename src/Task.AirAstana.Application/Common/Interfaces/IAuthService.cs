namespace Task.AirAstana.Application.Common.Interfaces;

public interface IAuthService
{
    string GenerateJwtToken(string userId, string username, string role);
    Task<bool> ValidateTokenAsync(string token);
}