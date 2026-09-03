using DeployNexus.Domain.Entities;
using DeployNexus.Infrastructure.Data;
using DeployNexus.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeployNexus.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DeployNexusDbContext _context;


    public UserRepository(
        DeployNexusDbContext context)
    {
        _context = context;
    }


    // ============================================================
    // CREATE
    // ============================================================

    public async Task<User> AddAsync(
        User user)
    {
        await _context.Users.AddAsync(user);

        return user;
    }


    // ============================================================
    // GET BY ID - SYSTEM ACCESS
    // ============================================================

    public async Task<User?> GetByIdAsync(
        Guid id)
    {
        return await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    // ============================================================
    // GET BY ID - ORGANIZATION SCOPED
    // ============================================================

    public async Task<User?> GetByIdAsync(
        Guid id,
        Guid organizationId)
    {
        return await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.OrganizationId == organizationId);
    }


    // ============================================================
    // GET ALL - SYSTEM ACCESS
    // ============================================================

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Where(x => x.IsActive)
            .Include(x => x.Role)
            .ToListAsync();
    }


    // ============================================================
    // GET ALL - ORGANIZATION SCOPED
    // ============================================================

    public async Task<IEnumerable<User>> GetAllAsync(
        Guid organizationId)
    {
        return await _context.Users
            .Where(x =>
                x.IsActive &&
                x.OrganizationId == organizationId)
            .Include(x => x.Role)
            .ToListAsync();
    }


    // ============================================================
    // GET INACTIVE - SYSTEM ACCESS
    // ============================================================

    public async Task<IEnumerable<User>>
        GetInactiveUsersAsync()
    {
        return await _context.Users
            .Where(x => !x.IsActive)
            .Include(x => x.Role)
            .ToListAsync();
    }


    // ============================================================
    // GET INACTIVE - ORGANIZATION SCOPED
    // ============================================================

    public async Task<IEnumerable<User>>
        GetInactiveUsersAsync(
            Guid organizationId)
    {
        return await _context.Users
            .Where(x =>
                !x.IsActive &&
                x.OrganizationId == organizationId)
            .Include(x => x.Role)
            .ToListAsync();
    }


    // ============================================================
    // GET BY USERNAME
    // ============================================================

    public async Task<User?> GetByUsernameAsync(
        string username)
    {
        return await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x =>
                x.Username == username);
    }


    // ============================================================
    // USERNAME EXISTS
    // ============================================================

    public async Task<bool> ExistsByUsernameAsync(
        Guid organizationId,
        string username,
        Guid? excludeUserId = null)
    {
        return await _context.Users
            .AnyAsync(x =>
                x.OrganizationId == organizationId &&
                x.Username == username &&
                (!excludeUserId.HasValue ||
                 x.Id != excludeUserId.Value));
    }


    // ============================================================
    // EMAIL EXISTS
    // ============================================================

    public async Task<bool> ExistsByEmailAsync(
        Guid organizationId,
        string email,
        Guid? excludeUserId = null)
    {
        return await _context.Users
            .AnyAsync(x =>
                x.OrganizationId == organizationId &&
                x.Email == email &&
                (!excludeUserId.HasValue ||
                 x.Id != excludeUserId.Value));
    }


    // ============================================================
    // UPDATE
    // ============================================================

    public Task UpdateAsync(
        User user)
    {
        _context.Users.Update(user);

        return Task.CompletedTask;
    }


    // ============================================================
    // SAVE
    // ============================================================

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}