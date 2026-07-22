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

        var result = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive
        };

        return Task.FromResult(result);
    }


    public Task<UserDto?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }


    public Task<IEnumerable<UserDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }


    public Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        throw new NotImplementedException();
    }


    public Task<Result> DeactivateAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}