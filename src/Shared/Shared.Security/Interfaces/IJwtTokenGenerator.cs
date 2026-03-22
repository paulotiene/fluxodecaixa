namespace Shared.Security.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userName, string role);
}