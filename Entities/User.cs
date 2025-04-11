using Microsoft.AspNetCore.Identity;
namespace BlogProject.Entities;
public class User : IdentityUser
{
    
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public List<Post> Posts { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public string? ProfileImage { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
