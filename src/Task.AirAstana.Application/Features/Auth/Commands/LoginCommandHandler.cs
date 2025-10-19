using MediatR;
using Task.AirAstana.Application.Common.Interfaces;
using Task.AirAstana.Application.Features.Auth.Models;
using Task.AirAstana.Domain.Exceptions;
using Task.AirAstana.Domain.Repositories;

namespace Task.AirAstana.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthService _authService;

    public LoginCommandHandler( IUserRepository userRepository, IPasswordHasher passwordHasher, IAuthService authService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authService = authService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new DomainException("Неверное имя пользователя или пароль");

        var token = _authService.GenerateJwtToken(
            user.Id.ToString(),
            user.Username,
            user.Role.Code);

        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.Code,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }
}