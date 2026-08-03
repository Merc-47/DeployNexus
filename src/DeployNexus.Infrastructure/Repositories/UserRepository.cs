using DeployNexus.Domain.Entities;
using DeployNexus.Infrastructure.Data;
using DeployNexus.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DeployNexusDbContext _context;


    public UserRepository(DeployNexusDbContext context)
    {
        _context = context;
    }


    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);

        return user;
    }


    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Where(x => x.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetInactiveUsersAsync()
    {
        return await _context.Users
            .Where(x => !x.IsActive)
            .ToListAsync();
    }

    public Task UpdateAsync(User user)
    {
        _context.Users.Update(user);

        return Task.CompletedTask;
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}