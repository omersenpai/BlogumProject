using System.ComponentModel.DataAnnotations;
using BlogProject.Entities;

namespace BlogProject.Models
{
 
    public class PostListViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string Category { get; set; }
        public string SearchQuery { get; set; }
    }

  
    public class PostCreateEditViewModel
    {
        [Required(ErrorMessage = "Başlık alanı zorunludur")]
        [StringLength(100, ErrorMessage = "Başlık en fazla {1} karakter olabilir")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İçerik alanı zorunludur")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Açıklama alanı zorunludur")]
        [StringLength(200, ErrorMessage = "Açıklama en fazla {1} karakter olabilir")]
        public string Description { get; set; }

        public string Image { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        // Form'a kategorileri seçmek için kullanılacak
        public IEnumerable<Category> Categories { get; set; }
    }


    public class PostDetailViewModel
    {
        public Post Post { get; set; }
        
        // Yeni yorum için form alanı
        [Required(ErrorMessage = "Yorum içeriği zorunludur")]
        [StringLength(1000, ErrorMessage = "Yorum en fazla {1} karakter olabilir")]
        public string CommentContent { get; set; }
    }
} 