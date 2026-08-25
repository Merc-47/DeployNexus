using DeployNexus.Application.Authentication.DTOs;
using DeployNexus.Application.Authentication.Interfaces;
using DeployNexus.Application.Common.Exceptions;
using DeployNexus.Application.Common.Interfaces;

namespace DeployNexus.Application.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }


    public async Task<LoginResponse> LoginAsync(
        LoginRequest request)
    {
        if (request == null)
        {
            throw new ValidationException(
                "Login request is required");
        }

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ValidationException(
                "Username is required");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException(
                "Password is required");
        }


        var user =
            await _userRepository.GetByUsernameAsync(
                request.Username);


        if (user == null)
        {
            // Do not reveal whether the username exists.
            throw new UnauthorizedAccessException(
                "Invalid username or password");
        }


        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "User account is inactive");
        }


        var passwordValid =
            _passwordHasher.Verify(
                request.Password,
                user.PasswordHash);


        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password");
        }


        return _jwtTokenGenerator.GenerateToken(user);
    }
}