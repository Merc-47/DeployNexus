using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User> AddAsync(User user);

    Task<User?> GetByIdAsync(Guid id);

    Task<IEnumerable<User>> GetAllAsync();

    Task<IEnumerable<User>> GetInactiveUsersAsync();

    Task<User?> GetByUsernameAsync(string username);

    Task<bool> ExistsByUsernameAsync(
    Guid organizationId,
    string username,
    Guid? excludeUserId = null);

    Task<bool> ExistsByEmailAsync(
        Guid organizationId,
        string email,
        Guid? excludeUserId = null);

    Task UpdateAsync(User user);

    Task SaveChangesAsync();
}