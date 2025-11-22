using System.Security.Claims;
using Backend.Controllers;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Backend.Tests;

public class BooksControllerTests
{
    private static AppDbContext CreateContext(string name)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new AppDbContext(options);
    }

    private static BooksController CreateController(AppDbContext context, int userId = 1)
    {
        var controller = new BooksController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }, "mock"))
                }
            }
        };

        return controller;
    }

    [Fact]
    public async Task GetBooks_ReturnsOnlyUserBooks()
    {
        await using var context = CreateContext(nameof(GetBooks_ReturnsOnlyUserBooks));
        context.Books.AddRange(
            new Book { Id = 1, Title = "Mine", Author = "Author", PublicationDate = DateTime.Today, UserId = 1 },
            new Book { Id = 2, Title = "Theirs", Author = "Author", PublicationDate = DateTime.Today, UserId = 2 }
        );
        await context.SaveChangesAsync();
        var controller = CreateController(context, userId: 1);

        var result = await controller.GetBooks();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var books = Assert.IsAssignableFrom<IEnumerable<BookDto>>(ok.Value);
        var book = Assert.Single(books);
        Assert.Equal(1, book.Id);
        Assert.Equal("Mine", book.Title);
    }

    [Fact]
    public async Task CreateBook_SavesBookForUser()
    {
        await using var context = CreateContext(nameof(CreateBook_SavesBookForUser));
        var controller = CreateController(context, userId: 5);
        var dto = new BookDto
        {
            Title = "New Book",
            Author = "Tester",
            PublicationDate = new DateTime(2020, 1, 1)
        };

        var response = await controller.CreateBook(dto);

        var created = Assert.IsType<CreatedAtActionResult>(response);
        var saved = context.Books.Single();
        Assert.Equal(5, saved.UserId);
        Assert.Equal("New Book", saved.Title);
        Assert.NotNull(dto.Id);
        Assert.Equal(saved.Id, dto.Id);
    }

    [Fact]
    public async Task UpdateBook_ReturnsNotFound_WhenBookMissing()
    {
        await using var context = CreateContext(nameof(UpdateBook_ReturnsNotFound_WhenBookMissing));
        var controller = CreateController(context, userId: 3);

        var result = await controller.UpdateBook(99, new BookDto { Title = "T", Author = "A", PublicationDate = DateTime.Today });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteBook_RemovesBook_WhenOwnedByUser()
    {
        await using var context = CreateContext(nameof(DeleteBook_RemovesBook_WhenOwnedByUser));
        context.Books.Add(new Book { Id = 7, Title = "Mine", Author = "A", PublicationDate = DateTime.Today, UserId = 2 });
        await context.SaveChangesAsync();
        var controller = CreateController(context, userId: 2);

        var result = await controller.DeleteBook(7);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Books);
    }
}