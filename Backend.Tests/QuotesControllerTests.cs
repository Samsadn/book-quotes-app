using System.Collections.Generic;
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

public class QuotesControllerTests
{
    private static AppDbContext CreateContext(string name)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new AppDbContext(options);
    }

    private static QuotesController CreateController(AppDbContext context, int userId = 1)
    {
        var controller = new QuotesController(context)
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
    public async Task GetQuotes_ReturnsOnlyUserQuotes()
    {
        await using var context = CreateContext(nameof(GetQuotes_ReturnsOnlyUserQuotes));
        context.Quotes.AddRange(
            new Quote { Id = 1, Text = "Mine", Author = "Me", UserId = 1 },
            new Quote { Id = 2, Text = "Theirs", Author = "Them", UserId = 2 }
        );
        await context.SaveChangesAsync();
        var controller = CreateController(context, userId: 1);

        var result = await controller.GetQuotes();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var quotes = Assert.IsAssignableFrom<IEnumerable<QuoteDto>>(ok.Value);
        var quote = Assert.Single(quotes);
        Assert.Equal(1, quote.Id);
        Assert.Equal("Mine", quote.Text);
    }

    [Fact]
    public async Task CreateQuote_SavesQuoteForUser()
    {
        await using var context = CreateContext(nameof(CreateQuote_SavesQuoteForUser));
        var controller = CreateController(context, userId: 3);
        var dto = new QuoteDto
        {
            Text = "A test quote",
            Author = "Tester"
        };

        var response = await controller.CreateQuote(dto);

        var created = Assert.IsType<CreatedAtActionResult>(response);
        Assert.Equal(nameof(QuotesController.GetQuotes), created.ActionName);
        var saved = context.Quotes.Single();
        Assert.Equal(3, saved.UserId);
        Assert.Equal("A test quote", saved.Text);
        Assert.NotNull(dto.Id);
        Assert.Equal(saved.Id, dto.Id);
    }

    [Fact]
    public async Task UpdateQuote_ReturnsNotFound_WhenQuoteMissing()
    {
        await using var context = CreateContext(nameof(UpdateQuote_ReturnsNotFound_WhenQuoteMissing));
        var controller = CreateController(context, userId: 4);

        var result = await controller.UpdateQuote(42, new QuoteDto { Text = "Updated", Author = "A" });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteQuote_RemovesQuote_WhenOwnedByUser()
    {
        await using var context = CreateContext(nameof(DeleteQuote_RemovesQuote_WhenOwnedByUser));
        context.Quotes.Add(new Quote { Id = 5, Text = "Mine", Author = "A", UserId = 2 });
        await context.SaveChangesAsync();
        var controller = CreateController(context, userId: 2);

        var result = await controller.DeleteQuote(5);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Quotes);
    }
}