using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployNexus.Application.Users.Interfaces;

using DeployNexus.Application.Common;
using DeployNexus.Application.Users.DTOs;

public interface IUserService
{
    Task<UserDto> CreateAsync(CreateUserRequest request);

    Task<UserDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<UserDto>> GetAllAsync();

    Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request);

    Task<Result> DeactivateAsync(Guid id);
}