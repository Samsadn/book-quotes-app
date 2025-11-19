namespace Backend.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    public ICollection<Book> Books { get; set; } = new List<Book>();
    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}