namespace Task.AirAstana.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IFlightRepository Flights { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}