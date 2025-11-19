namespace Backend.DTOs;

public class QuoteDto
{
    public int? Id { get; set; }
    public string Text { get; set; } = null!;
    public string? Author { get; set; }
}
