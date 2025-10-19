using Microsoft.EntityFrameworkCore;
using Task.AirAstana.Domain.Entities;
using Task.AirAstana.Domain.Repositories;

namespace Task.AirAstana.Infrastructure.Persistence.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly ApplicationDbContext _context;

    public FlightRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _context.Flights .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task<IEnumerable<Flight>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Flights .OrderBy(f => f.Arrival) .ToListAsync(cancellationToken);

    public async Task<Flight> AddAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        await _context.Flights.AddAsync(flight, cancellationToken);
        return flight;
    }

    public System.Threading.Tasks.Task UpdateAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        _context.Flights.Update(flight);
        return System.Threading.Tasks.Task.CompletedTask;
    }
    
}