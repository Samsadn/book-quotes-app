using System.Security.Cryptography;
using System.Text;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        var username = dto.UserName.ToLower();

        if (await _context.Users.AnyAsync(x => x.UserName == username))
            return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();

        var user = new User
        {
            UserName = username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        var username = dto.UserName.ToLower();

        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);
        if (user == null) return Unauthorized("Invalid username or password");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

        if (!computed.SequenceEqual(user.PasswordHash))
            return Unauthorized("Invalid username or password");

        var token = _tokenService.CreateToken(user);
        return Ok(new { token });
    }
}
