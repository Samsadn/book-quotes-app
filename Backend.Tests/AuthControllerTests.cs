using System.Security.Cryptography;
using System.Text;
using Backend.Controllers;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Backend.Tests;

public class AuthControllerTests
{
    private static AppDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
        return new AppDbContext(options);
    }

    private static TokenService CreateTokenService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Jwt:Key", "super_secret_key_for_testing_purposes"},
                {"Jwt:Issuer", "test-issuer"},
                {"Jwt:Audience", "test-audience"},
                {"Jwt:ExpiresMinutes", "30"}
            })
            .Build();

        return new TokenService(configuration);
    }

    [Fact]
    public async Task Register_AddsUser_WhenUsernameIsAvailable()
    {
        await using var context = CreateContext(nameof(Register_AddsUser_WhenUsernameIsAvailable));
        var controller = new AuthController(context, CreateTokenService());

        var result = await controller.Register(new RegisterDto { UserName = "NewUser", Password = "password" });

        Assert.IsType<OkResult>(result);
        Assert.Single(context.Users);
        Assert.Equal("newuser", context.Users.Single().UserName);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenUsernameIsTaken()
    {
        await using var context = CreateContext(nameof(Register_ReturnsBadRequest_WhenUsernameIsTaken));
        context.Users.Add(new User { UserName = "existing", PasswordHash = [1], PasswordSalt = [2] });
        await context.SaveChangesAsync();
        var controller = new AuthController(context, CreateTokenService());

        var result = await controller.Register(new RegisterDto { UserName = "Existing", Password = "password" });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Username is taken", badRequest.Value);
    }

    [Fact]
    public async Task Login_ReturnsToken_ForValidCredentials()
    {
        await using var context = CreateContext(nameof(Login_ReturnsToken_ForValidCredentials));
        using var hmac = new HMACSHA512();
        var password = "secret";
        var user = new User
        {
            UserName = "loginuser",
            PasswordSalt = hmac.Key,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password))
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var controller = new AuthController(context, CreateTokenService());

        var result = await controller.Login(new LoginDto { UserName = "LoginUser", Password = password });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var tokenProperty = okResult.Value?.GetType().GetProperty("token")?.GetValue(okResult.Value, null);
        var tokenString = Assert.IsType<string>(tokenProperty);
        Assert.False(string.IsNullOrWhiteSpace(tokenString));
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_ForInvalidCredentials()
    {
        await using var context = CreateContext(nameof(Login_ReturnsUnauthorized_ForInvalidCredentials));
        context.Users.Add(new User { UserName = "someone", PasswordHash = [1], PasswordSalt = [2] });
        await context.SaveChangesAsync();
        var controller = new AuthController(context, CreateTokenService());

        var result = await controller.Login(new LoginDto { UserName = "someone", Password = "wrong" });

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid username or password", unauthorized.Value);
    }
}