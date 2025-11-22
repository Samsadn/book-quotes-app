using Backend.Data;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/books")]
[Authorize] // Only authenticated users (valid JWT) can access CRUD operations
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;

    public BooksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
    {
        var userId = User.GetUserId();

        var books = await _context.Books
            .Where(b => b.UserId == userId)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                PublicationDate = b.PublicationDate
            })
            .ToListAsync();

        return Ok(books);
    }

    [HttpPost]
    public async Task<ActionResult> CreateBook(BookDto dto)
    {
        var userId = User.GetUserId();

        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            PublicationDate = dto.PublicationDate,
            UserId = userId
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        dto.Id = book.Id;
        return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateBook(int id, BookDto dto)
    {
        var userId = User.GetUserId();

        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        if (book == null) return NotFound();

        book.Title = dto.Title;
        book.Author = dto.Author;
        book.PublicationDate = dto.PublicationDate;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBook(int id)
    {
        var userId = User.GetUserId();

        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        if (book == null) return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
