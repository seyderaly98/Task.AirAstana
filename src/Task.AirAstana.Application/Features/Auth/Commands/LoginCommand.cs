using MediatR;
using Task.AirAstana.Application.Features.Auth.Models;

namespace Task.AirAstana.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<AuthResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}