using FluentValidation;

namespace Task.AirAstana.Application.Features.Auth.Commands;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя обязательно")
            .MaximumLength(20).WithMessage("Имя пользователя не должно превышать 20 символов");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов");
    }
}