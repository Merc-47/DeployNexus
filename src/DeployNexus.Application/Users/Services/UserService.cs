using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Application.Users.DTOs;
using DeployNexus.Application.Users.Interfaces;
using DeployNexus.Application.Authentication.Interfaces;
using DeployNexus.Domain.Entities;

namespace DeployNexus.Application.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;


    public UserService(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }


    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var organization = await _organizationRepository
            .GetByIdAsync(request.OrganizationId);

        if (organization == null)
        {
            throw new Exception("Organization not found");
        }

        if (!organization.IsActive)
        {
            throw new Exception("Organization is inactive");
        }


        var usernameExists = await _userRepository
            .ExistsByUsernameAsync(
                request.OrganizationId,
                request.Username);

        if (usernameExists)
        {
            throw new Exception(
                "A user with this username already exists");
        }


        var emailExists = await _userRepository
            .ExistsByEmailAsync(
                request.OrganizationId,
                request.Email);

        if (emailExists)
        {
            throw new Exception(
                "A user with this email already exists");
        }


        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true,
            OrganizationId = request.OrganizationId,
            RoleId = null
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
        var users = await _userRepository
            .GetInactiveUsersAsync();

        return users.Select(MapToDto);
    }


    public async Task<UserDto?> UpdateAsync(
        Guid id,
        UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return null;
        }


        var organization = await _organizationRepository
            .GetByIdAsync(request.OrganizationId);

        if (organization == null)
        {
            throw new Exception("Organization not found");
        }

        if (!organization.IsActive)
        {
            throw new Exception("Organization is inactive");
        }


        // Check username only if it is being changed.
        if (!string.Equals(
                user.Username,
                request.Username,
                StringComparison.OrdinalIgnoreCase))
        {
            var usernameExists = await _userRepository
                .ExistsByUsernameAsync(
                    request.OrganizationId,
                    request.Username);

            if (usernameExists)
            {
                throw new Exception(
                    "A user with this username already exists");
            }
        }


        // Check email only if it is being changed.
        if (!string.Equals(
                user.Email,
                request.Email,
                StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _userRepository
                .ExistsByEmailAsync(
                    request.OrganizationId,
                    request.Email);

            if (emailExists)
            {
                throw new Exception(
                    "A user with this email already exists");
            }
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


    public async Task<UserDto?> AssignRoleAsync(
     Guid userId,
     AssignUserRoleRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return null;
        }

        // Null RoleId means remove the user's role.
        if (request.RoleId == null)
        {
            user.RoleId = null;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToDto(user);
        }

        var role = await _roleRepository
            .GetByIdAsync(request.RoleId.Value);

        if (role == null)
        {
            throw new Exception("Role not found");
        }

        if (!role.IsActive)
        {
            throw new Exception("Role is inactive");
        }

        if (role.OrganizationId != user.OrganizationId)
        {
            throw new Exception(
                "Role does not belong to the user's organization");
        }

        user.RoleId = role.Id;

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
            OrganizationId = user.OrganizationId,
            RoleId = user.RoleId
        };
    }
}