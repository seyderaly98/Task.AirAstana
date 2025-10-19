using Task.AirAstana.Domain.Entities;

namespace Task.AirAstana.Domain.Repositories;

public interface IFlightRepository
{
    Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Flight>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Flight> AddAsync(Flight flight, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task UpdateAsync(Flight flight, CancellationToken cancellationToken = default);
}