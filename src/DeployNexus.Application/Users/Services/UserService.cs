using DeployNexus.Application.Common;
using DeployNexus.Application.Common.Exceptions;
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
    private readonly ICurrentUserService _currentUserService;


    public UserService(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }


    // ============================================================
    // CREATE USER
    // ============================================================

    public async Task<UserDto> CreateAsync(
        CreateUserRequest request)
    {
        // --------------------------------------------------------
        // Organization access
        // --------------------------------------------------------
        //
        // System users can create users in any organization.
        //
        // Organization users can only create users inside
        // their own organization.
        // --------------------------------------------------------

        EnsureOrganizationAccess(
            request.OrganizationId);


        // --------------------------------------------------------
        // Validate organization
        // --------------------------------------------------------

        var organization =
            await _organizationRepository
                .GetByIdAsync(
                    request.OrganizationId);

        if (organization == null)
        {
            throw new NotFoundException(
                "Organization not found");
        }

        if (!organization.IsActive)
        {
            throw new ValidationException(
                "Organization is inactive");
        }


        // --------------------------------------------------------
        // Username uniqueness
        // --------------------------------------------------------

        var usernameExists =
            await _userRepository
                .ExistsByUsernameAsync(
                    request.OrganizationId,
                    request.Username);

        if (usernameExists)
        {
            throw new ConflictException(
                "A user with this username already exists");
        }


        // --------------------------------------------------------
        // Email uniqueness
        // --------------------------------------------------------

        var emailExists =
            await _userRepository
                .ExistsByEmailAsync(
                    request.OrganizationId,
                    request.Email);

        if (emailExists)
        {
            throw new ConflictException(
                "A user with this email already exists");
        }


        // --------------------------------------------------------
        // Create user
        // --------------------------------------------------------

        var user = new User
        {
            Id = Guid.NewGuid(),

            Username = request.Username,

            Email = request.Email,

            PasswordHash =
                _passwordHasher.Hash(
                    request.Password),

            FirstName = request.FirstName,

            LastName = request.LastName,

            IsActive = true,

            OrganizationId =
                request.OrganizationId,

            // New users have no role by default.
            RoleId = null
        };


        await _userRepository
            .AddAsync(user);

        await _userRepository
            .SaveChangesAsync();


        return MapToDto(user);
    }


    // ============================================================
    // GET USER BY ID
    // ============================================================

    public async Task<UserDto?> GetByIdAsync(
        Guid id)
    {
        User? user;


        // --------------------------------------------------------
        // System user
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            user =
                await _userRepository
                    .GetByIdAsync(id);
        }


        // --------------------------------------------------------
        // Organization user
        // --------------------------------------------------------

        else
        {
            var organizationId =
                GetCurrentOrganizationId();

            user =
                await _userRepository
                    .GetByIdAsync(
                        id,
                        organizationId);
        }


        if (user == null)
        {
            return null;
        }


        return MapToDto(user);
    }


    // ============================================================
    // GET ALL USERS
    // ============================================================

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        IEnumerable<User> users;


        // --------------------------------------------------------
        // System user
        // --------------------------------------------------------
        //
        // System users can see users from every organization.
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            users =
                await _userRepository
                    .GetAllAsync();
        }


        // --------------------------------------------------------
        // Organization user
        // --------------------------------------------------------
        //
        // Organization users can only see users from their
        // own organization.
        // --------------------------------------------------------

        else
        {
            var organizationId =
                GetCurrentOrganizationId();

            users =
                await _userRepository
                    .GetAllAsync(
                        organizationId);
        }


        return users.Select(MapToDto);
    }


    // ============================================================
    // GET INACTIVE USERS
    // ============================================================

    public async Task<IEnumerable<UserDto>>
        GetInactiveUsersAsync()
    {
        IEnumerable<User> users;


        // --------------------------------------------------------
        // System user
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            users =
                await _userRepository
                    .GetInactiveUsersAsync();
        }


        // --------------------------------------------------------
        // Organization user
        // --------------------------------------------------------

        else
        {
            var organizationId =
                GetCurrentOrganizationId();

            users =
                await _userRepository
                    .GetInactiveUsersAsync(
                        organizationId);
        }


        return users.Select(MapToDto);
    }


    // ============================================================
    // UPDATE USER
    // ============================================================

    public async Task<UserDto?> UpdateAsync(
        Guid id,
        UpdateUserRequest request)
    {
        User? user;


        // --------------------------------------------------------
        // Find target user
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            user =
                await _userRepository
                    .GetByIdAsync(id);
        }
        else
        {
            var currentOrganizationId =
                GetCurrentOrganizationId();

            user =
                await _userRepository
                    .GetByIdAsync(
                        id,
                        currentOrganizationId);
        }


        if (user == null)
        {
            return null;
        }


        // --------------------------------------------------------
        // Organization access
        // --------------------------------------------------------
        //
        // System users can move users between organizations.
        //
        // Organization users cannot move a user outside their
        // own organization.
        // --------------------------------------------------------

        EnsureOrganizationAccess(
            request.OrganizationId);


        // --------------------------------------------------------
        // Validate target organization
        // --------------------------------------------------------

        var organization =
            await _organizationRepository
                .GetByIdAsync(
                    request.OrganizationId);

        if (organization == null)
        {
            throw new NotFoundException(
                "Organization not found");
        }

        if (!organization.IsActive)
        {
            throw new ValidationException(
                "Organization is inactive");
        }


        // --------------------------------------------------------
        // Username uniqueness
        // --------------------------------------------------------

        if (!string.Equals(
                user.Username,
                request.Username,
                StringComparison.OrdinalIgnoreCase))
        {
            var usernameExists =
                await _userRepository
                    .ExistsByUsernameAsync(
                        request.OrganizationId,
                        request.Username,
                        id);

            if (usernameExists)
            {
                throw new ConflictException(
                    "A user with this username already exists");
            }
        }


        // --------------------------------------------------------
        // Email uniqueness
        // --------------------------------------------------------

        if (!string.Equals(
                user.Email,
                request.Email,
                StringComparison.OrdinalIgnoreCase))
        {
            var emailExists =
                await _userRepository
                    .ExistsByEmailAsync(
                        request.OrganizationId,
                        request.Email,
                        id);

            if (emailExists)
            {
                throw new ConflictException(
                    "A user with this email already exists");
            }
        }


        // --------------------------------------------------------
        // Update user
        // --------------------------------------------------------

        user.Username =
            request.Username;

        user.Email =
            request.Email;

        user.FirstName =
            request.FirstName;

        user.LastName =
            request.LastName;

        user.OrganizationId =
            request.OrganizationId;


        await _userRepository
            .UpdateAsync(user);

        await _userRepository
            .SaveChangesAsync();


        return MapToDto(user);
    }


    // ============================================================
    // ASSIGN / REMOVE USER ROLE
    // ============================================================

    public async Task<UserDto?> AssignRoleAsync(
        Guid userId,
        AssignUserRoleRequest request)
    {
        User? user;


        // --------------------------------------------------------
        // Find target user
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            user =
                await _userRepository
                    .GetByIdAsync(userId);
        }
        else
        {
            var currentOrganizationId =
                GetCurrentOrganizationId();

            user =
                await _userRepository
                    .GetByIdAsync(
                        userId,
                        currentOrganizationId);
        }


        if (user == null)
        {
            return null;
        }


        // --------------------------------------------------------
        // Remove role
        // --------------------------------------------------------

        if (request.RoleId == null)
        {
            user.RoleId = null;

            await _userRepository
                .UpdateAsync(user);

            await _userRepository
                .SaveChangesAsync();

            return MapToDto(user);
        }


        // --------------------------------------------------------
        // Find role
        // --------------------------------------------------------

        var role =
            await _roleRepository
                .GetByIdAsync(
                    request.RoleId.Value);

        if (role == null)
        {
            throw new NotFoundException(
                "Role not found");
        }


        // --------------------------------------------------------
        // Role must be active
        // --------------------------------------------------------

        if (!role.IsActive)
        {
            throw new ValidationException(
                "Role is inactive");
        }


        // --------------------------------------------------------
        // Role must belong to the same organization
        // as the target user.
        // --------------------------------------------------------

        if (role.OrganizationId != user.OrganizationId)
        {
            throw new ValidationException(
                "Role does not belong to the user's organization");
        }


        // --------------------------------------------------------
        // Organization access
        // --------------------------------------------------------
        //
        // System users can assign roles across organizations.
        //
        // Organization users can only assign roles inside
        // their own organization.
        // --------------------------------------------------------

        EnsureOrganizationAccess(
            user.OrganizationId);


        // --------------------------------------------------------
        // Assign role
        // --------------------------------------------------------

        user.RoleId =
            role.Id;


        await _userRepository
            .UpdateAsync(user);

        await _userRepository
            .SaveChangesAsync();


        return MapToDto(user);
    }


    // ============================================================
    // DEACTIVATE USER
    // ============================================================

    public async Task<Result> DeactivateAsync(
        Guid id)
    {
        User? user;


        // --------------------------------------------------------
        // Find target user
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            user =
                await _userRepository
                    .GetByIdAsync(id);
        }
        else
        {
            var organizationId =
                GetCurrentOrganizationId();

            user =
                await _userRepository
                    .GetByIdAsync(
                        id,
                        organizationId);
        }


        if (user == null)
        {
            return Result.Failure(
                "User not found");
        }


        // --------------------------------------------------------
        // Organization access
        // --------------------------------------------------------

        EnsureOrganizationAccess(
            user.OrganizationId);


        // --------------------------------------------------------
        // Deactivate
        // --------------------------------------------------------

        if (!user.IsActive)
        {
            return Result.Failure(
                "User is already inactive");
        }


        user.IsActive = false;


        await _userRepository
            .UpdateAsync(user);

        await _userRepository
            .SaveChangesAsync();


        return Result.Ok();
    }


    // ============================================================
    // ORGANIZATION ACCESS
    // ============================================================

    private void EnsureOrganizationAccess(
        Guid organizationId)
    {
        // --------------------------------------------------------
        // System users can access every organization.
        // --------------------------------------------------------

        if (_currentUserService.IsSystemUser)
        {
            return;
        }


        // --------------------------------------------------------
        // User must be authenticated.
        // --------------------------------------------------------

        if (!_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated");
        }


        // --------------------------------------------------------
        // Organization must be present in the JWT.
        // --------------------------------------------------------

        if (!_currentUserService.OrganizationId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User organization could not be determined");
        }


        // --------------------------------------------------------
        // Organization user can only access their own
        // organization.
        // --------------------------------------------------------

        if (_currentUserService.OrganizationId.Value
            != organizationId)
        {
            throw new UnauthorizedAccessException(
                "You cannot access resources outside your organization");
        }
    }


    // ============================================================
    // CURRENT ORGANIZATION
    // ============================================================

    private Guid GetCurrentOrganizationId()
    {
        if (!_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated");
        }


        if (!_currentUserService.OrganizationId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User organization could not be determined");
        }


        return _currentUserService.OrganizationId.Value;
    }


    // ============================================================
    // MAPPING
    // ============================================================

    private static UserDto MapToDto(
        User user)
    {
        return new UserDto
        {
            Id = user.Id,

            Username = user.Username,

            Email = user.Email,

            FirstName = user.FirstName,

            LastName = user.LastName,

            IsActive = user.IsActive,

            OrganizationId =
                user.OrganizationId,

            RoleId =
                user.RoleId
        };
    }
}
