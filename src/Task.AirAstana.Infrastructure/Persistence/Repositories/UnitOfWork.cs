using Microsoft.EntityFrameworkCore.Storage;
using Task.AirAstana.Domain.Repositories;

namespace Task.AirAstana.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork( ApplicationDbContext context, IFlightRepository flights )
    {
        _context = context;
        Flights = flights;
    }

    public IFlightRepository Flights { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}