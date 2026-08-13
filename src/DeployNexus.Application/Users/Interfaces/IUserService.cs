using DeployNexus.Application.Common;
using DeployNexus.Application.Users.DTOs;

namespace DeployNexus.Application.Users.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateAsync(CreateUserRequest request);

    Task<UserDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<UserDto>> GetAllAsync();

    Task<IEnumerable<UserDto>> GetInactiveUsersAsync();

    Task<UserDto?> UpdateAsync(
        Guid id,
        UpdateUserRequest request);

    Task<UserDto?> AssignRoleAsync(
        Guid userId,
        AssignUserRoleRequest request);

    Task<Result> DeactivateAsync(Guid id);
}