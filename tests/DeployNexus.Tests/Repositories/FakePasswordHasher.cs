using DeployNexus.Application.Authentication.Interfaces;

namespace DeployNexus.Tests.Authentication;

public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return $"HASHED_{password}";
    }

    public bool Verify(string password, string passwordHash)
    {
        return passwordHash == $"HASHED_{password}";
    }
}