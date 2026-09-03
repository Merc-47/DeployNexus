using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using DeployNexus.Application.Authentication.DTOs;
using DeployNexus.Application.Authentication.Interfaces;
using DeployNexus.Domain.Entities;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DeployNexus.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(
        IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public LoginResponse GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            // ----------------------------------------------------
            // Identity
            // ----------------------------------------------------

            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new(
                ClaimTypes.Name,
                user.Username),

            new(
                ClaimTypes.Email,
                user.Email),

            // ----------------------------------------------------
            // Organization
            // ----------------------------------------------------

            new(
                "organizationId",
                user.OrganizationId.ToString())
        };


        // --------------------------------------------------------
        // Role information
        // --------------------------------------------------------

        if (user.RoleId.HasValue)
        {
            claims.Add(
                new Claim(
                    "roleId",
                    user.RoleId.Value.ToString()));


            // Role should already be loaded by authentication.
            if (user.Role != null)
            {
                claims.Add(
                    new Claim(
                        "roleType",
                        ((int)user.Role.RoleType).ToString()));
            }
        }


        // --------------------------------------------------------
        // Create signing key
        // --------------------------------------------------------

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _settings.SecretKey));


        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);


        var expires =
            DateTime.UtcNow.AddMinutes(
                _settings.ExpirationMinutes);


        // --------------------------------------------------------
        // Generate JWT
        // --------------------------------------------------------

        var token =
            new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);


        return new LoginResponse
        {
            Token =
                new JwtSecurityTokenHandler()
                    .WriteToken(token),

            ExpiresAt = expires
        };
    }
}