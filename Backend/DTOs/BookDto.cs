namespace Backend.DTOs;

public class BookDto
{
    public int? Id { get; set; }   // null when creating
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public DateTime PublicationDate { get; set; }
}
