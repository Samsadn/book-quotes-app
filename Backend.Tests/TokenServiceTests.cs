using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Models;
using Backend.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Backend.Tests;

public class TokenServiceTests
{
    private static TokenService CreateService(out IConfiguration configuration)
    {
        var settings = new Dictionary<string, string?>
        {
            {"Jwt:Key", "my_super_secret_key_for_testing_purposes"},
            {"Jwt:Issuer", "test-issuer"},
            {"Jwt:Audience", "test-audience"},
            {"Jwt:ExpiresMinutes", "30"}
        };

        configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();

        return new TokenService(configuration);
    }

    [Fact]
    public void CreateToken_EmbedsUserClaims()
    {
        var tokenService = CreateService(out _);
        var user = new User { Id = 42, UserName = "testUser" };

        var token = tokenService.CreateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal("test-issuer", jwt.Issuer);
        Assert.Equal("test-audience", jwt.Audiences.Single());
        Assert.Contains(jwt.Claims, c => (c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier) && c.Value == "42");
        Assert.Contains(jwt.Claims, c => (c.Type == "unique_name" || c.Type == ClaimTypes.Name) && c.Value == "testUser");
    }

    [Fact]
    public void CreateToken_HonorsExpiryConfiguration()
    {
        var tokenService = CreateService(out var configuration);
        var user = new User { Id = 1, UserName = "expires" };

        var before = DateTime.UtcNow.AddMinutes(int.Parse(configuration["Jwt:ExpiresMinutes"]!));
        var token = tokenService.CreateToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.InRange(jwt.ValidTo, before.AddSeconds(-30), before.AddSeconds(30));
    }
}