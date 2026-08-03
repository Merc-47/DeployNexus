using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Common.Interfaces;
public interface IUserRepository
{
    Task<User> AddAsync(User user);

    Task<User?> GetByIdAsync(Guid id);

    Task<IEnumerable<User>> GetAllAsync();

    Task<IEnumerable<User>> GetInactiveUsersAsync();

    Task UpdateAsync(User user);

    Task SaveChangesAsync();
}