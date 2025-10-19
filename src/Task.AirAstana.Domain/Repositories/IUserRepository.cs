using Task.AirAstana.Domain.Entities;

namespace Task.AirAstana.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}