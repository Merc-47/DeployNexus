using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Tests.Repositories;

public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new();


    public Task<User> AddAsync(User user)
    {
        _users.Add(user);

        return Task.FromResult(user);
    }


    public Task<User?> GetByIdAsync(Guid id)
    {
        var user = _users.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(user);
    }


    public Task<IEnumerable<User>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<User>>(_users);
    }


    public Task UpdateAsync(User user)
    {
        var existingUser = _users.FirstOrDefault(x => x.Id == user.Id);

        if (existingUser != null)
        {
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.IsActive = user.IsActive;
        }

        return Task.CompletedTask;
    }


    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}