using Task.AirAstana.Domain.Common;
using Task.AirAstana.Domain.Enums;
using Task.AirAstana.Domain.Exceptions;

namespace Task.AirAstana.Domain.Entities;

public class Flight : BaseAuditableEntity
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTimeOffset Departure { get; set; }
    public DateTimeOffset Arrival { get; set; }
    public FlightStatus Status { get; set; }

    public void UpdateStatus(FlightStatus newStatus)
    {
        if (Status == FlightStatus.Cancelled)
            throw new DomainException("Не удается обновить статус отмененного рейса"); 
        if(Status == newStatus)
            throw new DomainException($"Не удается обновить статус. Текущий статус [{Status}]"); 
        Status = newStatus;
    }

    public void Cancel()
    {
        Status = FlightStatus.Cancelled;
    }

    public void Delay()
    {
        if (Status != FlightStatus.Cancelled)
        {
            Status = FlightStatus.Delayed;
        }
    }
}