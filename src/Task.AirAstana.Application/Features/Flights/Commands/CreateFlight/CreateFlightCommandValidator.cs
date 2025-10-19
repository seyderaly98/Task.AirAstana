using FluentValidation;
using Task.AirAstana.Domain.Enums;

namespace Task.AirAstana.Application.Features.Flights.Commands.CreateFlight;

public class CreateFlightCommandValidator : AbstractValidator<CreateFlightCommand>
{
    public CreateFlightCommandValidator()
    {
        RuleFor(x => x.Origin)
            .NotEmpty().WithMessage("Пункт отправления обязателен")
            .MaximumLength(256).WithMessage("Пункт отправления не должен превышать 256 символов");

        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("Пункт назначения обязателен")
            .MaximumLength(256).WithMessage("Пункт назначения не должен превышать 256 символов")
            .NotEqual(x => x.Origin).WithMessage("Пункт назначения не может совпадать с пунктом отправления");

        RuleFor(x => x.Departure)
            .NotEmpty().WithMessage("Время вылета обязательно")
            .GreaterThan(DateTimeOffset.UtcNow).WithMessage("Время вылета должно быть в будущем");

        RuleFor(x => x.Arrival)
            .NotEmpty().WithMessage("Время прилёта обязательно")
            .GreaterThan(x => x.Departure).WithMessage("Время прилёта должно быть после времени вылета");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Некорректный статус рейса")
            .NotEqual(FlightStatus.Cancelled).WithMessage("Нельзя создать рейс со статусом 'Отменён'");
    }
}