using FluentValidation;

namespace Task.AirAstana.Application.Features.Flights.Queries;

public class GetFlightsQueryValidator : AbstractValidator<GetFlightsQuery>
{
    public GetFlightsQueryValidator()
    {
        RuleFor(x => x.Origin)
            .MaximumLength(256).WithMessage("Пункт отправления не должен превышать 256 символов")
            .When(x => !string.IsNullOrWhiteSpace(x.Origin));

        RuleFor(x => x.Destination)
            .MaximumLength(256).WithMessage("Пункт назначения не должен превышать 256 символов")
            .When(x => !string.IsNullOrWhiteSpace(x.Destination));
    }
}