namespace BlogProject.Entities;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Url { get; set; } = null!;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
