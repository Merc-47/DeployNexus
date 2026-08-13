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


    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Username == username);
    }


    public async Task<bool> ExistsByUsernameAsync(
    Guid organizationId,
    string username,
    Guid? excludeUserId = null)
    {
        return await _context.Users.AnyAsync(x =>
            x.OrganizationId == organizationId &&
            x.Username == username &&
            (!excludeUserId.HasValue || x.Id != excludeUserId.Value));
    }


    public async Task<bool> ExistsByEmailAsync(
    Guid organizationId,
    string email,
    Guid? excludeUserId = null)
    {
        return await _context.Users.AnyAsync(x =>
            x.OrganizationId == organizationId &&
            x.Email == email &&
            (!excludeUserId.HasValue || x.Id != excludeUserId.Value));
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