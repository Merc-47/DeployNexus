using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeployNexus.Application.Users.DTOs;
using DeployNexus.Application.Users.Services;

namespace DeployNexus.Tests.Users;

public class UserServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var service = new UserService();

        var request = new CreateUserRequest
        {
            Username = "jason",
            Email = "jason@test.com",
            FirstName = "Jason",
            LastName = "Broody"
        };


        // Act
        var result = await service.CreateAsync(request);


        // Assert
        Assert.NotNull(result);
        Assert.Equal("jason", result.Username);
        Assert.Equal("jason@test.com", result.Email);
        Assert.True(result.IsActive);
    }
}
