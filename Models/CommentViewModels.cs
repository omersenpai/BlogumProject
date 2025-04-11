using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    
    public class CommentCreateViewModel
    {
        [Required(ErrorMessage = "Yorum içeriği zorunludur")]
        [StringLength(1000, ErrorMessage = "Yorum en fazla {1} karakter olabilir")]
        [Display(Name = "Yorumunuz")]
        public string Content { get; set; }

        public int PostId { get; set; }
    }

  
    public class UserCommentsViewModel
    {
        public IEnumerable<CommentListItemViewModel> Comments { get; set; }
        public string Username { get; set; }
    }

    public class CommentListItemViewModel
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PostTitle { get; set; }
        public int PostId { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
} 