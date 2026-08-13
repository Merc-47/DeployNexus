using DeployNexus.Domain.Entities;
using DeployNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeployNexus.Tests.Integration;

public class UserDatabaseConstraintTests
    : IClassFixture<DeployNexusWebApplicationFactory>
{
    private readonly DeployNexusWebApplicationFactory _factory;

    public UserDatabaseConstraintTests(
        DeployNexusWebApplicationFactory factory)
    {
        _factory = factory;
    }


    [Fact]
    public async Task Database_ShouldRejectDuplicateUsernameWithinSameOrganization()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<DeployNexusDbContext>();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var organizationId = Guid.NewGuid();

        var organization = new Organization
        {
            Id = organizationId,
            Name = $"Test Organization {Guid.NewGuid()}",
            Code = $"TEST-{Guid.NewGuid():N}"
        };

        context.Organizations.Add(organization);

        var firstUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "duplicate-user",
            Email = $"first-{Guid.NewGuid():N}@test.com",
            PasswordHash = "test-hash",
            FirstName = "First",
            LastName = "User",
            IsActive = true,
            OrganizationId = organizationId
        };

        context.Users.Add(firstUser);

        await context.SaveChangesAsync();


        // Act
        var secondUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "duplicate-user",
            Email = $"second-{Guid.NewGuid():N}@test.com",
            PasswordHash = "test-hash",
            FirstName = "Second",
            LastName = "User",
            IsActive = true,
            OrganizationId = organizationId
        };

        context.Users.Add(secondUser);


        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());

        await transaction.RollbackAsync();
    }


    [Fact]
    public async Task Database_ShouldRejectDuplicateEmailWithinSameOrganization()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<DeployNexusDbContext>();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var organizationId = Guid.NewGuid();

        var organization = new Organization
        {
            Id = organizationId,
            Name = $"Test Organization {Guid.NewGuid()}",
            Code = $"TEST-{Guid.NewGuid():N}"
        };

        context.Organizations.Add(organization);

        var duplicateEmail =
            $"duplicate-{Guid.NewGuid():N}@test.com";

        var firstUser = new User
        {
            Id = Guid.NewGuid(),
            Username = $"first-{Guid.NewGuid():N}",
            Email = duplicateEmail,
            PasswordHash = "test-hash",
            FirstName = "First",
            LastName = "User",
            IsActive = true,
            OrganizationId = organizationId
        };

        context.Users.Add(firstUser);

        await context.SaveChangesAsync();


        // Act
        var secondUser = new User
        {
            Id = Guid.NewGuid(),
            Username = $"second-{Guid.NewGuid():N}",
            Email = duplicateEmail,
            PasswordHash = "test-hash",
            FirstName = "Second",
            LastName = "User",
            IsActive = true,
            OrganizationId = organizationId
        };

        context.Users.Add(secondUser);


        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(
            () => context.SaveChangesAsync());

        await transaction.RollbackAsync();
    }


    [Fact]
    public async Task Database_ShouldAllowSameUsernameAndEmailInDifferentOrganizations()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<DeployNexusDbContext>();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var organizationOneId = Guid.NewGuid();
        var organizationTwoId = Guid.NewGuid();

        var organizationOne = new Organization
        {
            Id = organizationOneId,
            Name = $"Test Organization One {Guid.NewGuid()}",
            Code = $"TEST-{Guid.NewGuid():N}"
        };

        var organizationTwo = new Organization
        {
            Id = organizationTwoId,
            Name = $"Test Organization Two {Guid.NewGuid()}",
            Code = $"TEST-{Guid.NewGuid():N}"
        };

        context.Organizations.AddRange(
            organizationOne,
            organizationTwo);

        await context.SaveChangesAsync();


        var firstUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "same-user",
            Email = "same@test.com",
            PasswordHash = "test-hash",
            FirstName = "First",
            LastName = "User",
            IsActive = true,
            OrganizationId = organizationOneId
        };

        var secondUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "same-user",
            Email = "same@test.com",
            PasswordHash = "test-hash",
            FirstName = "Second",
            LastName = "User",
            IsActive = true,
            OrganizationId = organizationTwoId
        };

        context.Users.AddRange(
            firstUser,
            secondUser);


        // Act
        var exception =
            await Record.ExceptionAsync(
                () => context.SaveChangesAsync());


        // Assert
        Assert.Null(exception);

        await transaction.RollbackAsync();
    }
}