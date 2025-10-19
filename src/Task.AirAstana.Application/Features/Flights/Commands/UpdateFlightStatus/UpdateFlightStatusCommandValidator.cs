using FluentValidation;

namespace Task.AirAstana.Application.Features.Flights.Commands.UpdateFlightStatus;

public class UpdateFlightStatusCommandValidator : AbstractValidator<UpdateFlightStatusCommand>
{
    public UpdateFlightStatusCommandValidator()
    {
        RuleFor(x => x.FlightId).GreaterThan(0).WithMessage("ID рейса должен быть больше 0");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Некорректный статус рейса");
    }
}