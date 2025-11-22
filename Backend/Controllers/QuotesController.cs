using Backend.Data;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/quotes")]
[Authorize]
public class QuotesController : ControllerBase
{
    private readonly AppDbContext _context;

    public QuotesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetQuotes()
    {
        var userId = User.GetUserId();

        var quotes = await _context.Quotes
            .Where(q => q.UserId == userId)
            .Select(q => new QuoteDto
            {
                Id = q.Id,
                Text = q.Text,
                Author = q.Author
            })
            .ToListAsync();

        return Ok(quotes);
    }

    [HttpPost]
    public async Task<ActionResult> CreateQuote(QuoteDto dto)
    {
        var userId = User.GetUserId();

        var quote = new Quote
        {
            Text = dto.Text,
            Author = dto.Author,
            UserId = userId
        };

        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();

        dto.Id = quote.Id;
        return CreatedAtAction(nameof(GetQuotes), new { id = quote.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateQuote(int id, QuoteDto dto)
    {
        var userId = User.GetUserId();

        var quote = await _context.Quotes.FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);
        if (quote == null) return NotFound();

        quote.Text = dto.Text;
        quote.Author = dto.Author;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteQuote(int id)
    {
        var userId = User.GetUserId();

        var quote = await _context.Quotes.FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);
        if (quote == null) return NotFound();

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
