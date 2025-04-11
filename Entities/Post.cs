using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogProject.Entities;

public class Post
{
    public int PostId { get; set; }

    [Required(ErrorMessage = "Başlık alanı zorunludur")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
    public string Title { get; set; } = string.Empty; //nullable dan kaçmak için

    [Required(ErrorMessage = "İçerik alanı zorunludur")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Açıklama alanı zorunludur")]
    [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
    public string Description { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public DateTime PublishedOn { get; set; }
    public bool IsActive { get; set; }

    public string UserId { get; set; } = string.Empty;
    
    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required(ErrorMessage = "Kategori seçimi zorunludur")]
    public int CategoryId { get; set; }
    
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    // Yorumlar
    public List<Comment> Comments { get; set; } = new();
}
