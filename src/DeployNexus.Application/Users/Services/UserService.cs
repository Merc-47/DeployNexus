using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.Users.DTOs;
using DeployNexus.Application.Users.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;


    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true,
            OrganizationId = request.OrganizationId
        };

        await _userRepository.AddAsync(user);

        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }


    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return null;
        }

        return MapToDto(user);
    }


    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(MapToDto);
    }

    public async Task<IEnumerable<UserDto>> GetInactiveUsersAsync()
    {
        var users = await _userRepository.GetInactiveUsersAsync();

        return users.Select(MapToDto);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return null;
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.OrganizationId = request.OrganizationId;

        await _userRepository.UpdateAsync(user);

        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }


    public async Task<Result> DeactivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        user.IsActive = false;

        await _userRepository.UpdateAsync(user);

        await _userRepository.SaveChangesAsync();

        return Result.Ok();
    }


    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            OrganizationId = user.OrganizationId
        };
    }
}