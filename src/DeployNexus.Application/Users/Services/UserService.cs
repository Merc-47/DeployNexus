using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployNexus.Application.Users.Services;

using DeployNexus.Application.Common;
using DeployNexus.Application.Users.DTOs;
using DeployNexus.Application.Users.Interfaces;
using DeployNexus.Domain.Entities;

public class UserService : IUserService
{
    private readonly List<User> _users = new();


    public Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true
        };

        _users.Add(user);

        return Task.FromResult(MapToDto(user));
    }


    public Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = _users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            return Task.FromResult<UserDto?>(null);
        }

        return Task.FromResult<UserDto?>(MapToDto(user));
    }


    public Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = _users
            .Select(MapToDto)
            .ToList();

        return Task.FromResult<IEnumerable<UserDto>>(users);
    }


    public Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = _users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            return Task.FromResult<UserDto?>(null);
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        return Task.FromResult<UserDto?>(MapToDto(user));
    }


    public Task<Result> DeactivateAsync(Guid id)
    {
        var user = _users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            return Task.FromResult(
                Result.Failure("User not found")
            );
        }

        user.IsActive = false;

        return Task.FromResult(
            Result.Ok()
        );
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
            IsActive = user.IsActive
        };
    }
}