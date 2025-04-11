using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogProject.Data.Abstract;
using BlogProject.Entities;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace BlogProject.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<CommentController> _logger;

        public CommentController(
            ICommentRepository commentRepository,
            IPostRepository postRepository,
            ILogger<CommentController> logger)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _logger = logger;
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(string content, int postId)
        {
            if (string.IsNullOrEmpty(content))
            {
                return Json(new { success = false, message = "Yorum içeriği boş olamaz." });
            }

            // Post ID'nin geçerli olup olmadığını kontrol et
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return Json(new { success = false, message = "Geçersiz gönderi." });
            }

            try
            {
                // Kullanıcı ID'sini al
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // Yeni yorum oluştur
                var comment = new Comment
                {
                    Content = content,
                    PostId = postId,
                    UserId = userId,
                    CreatedAt = DateTime.Now
                };
                
                // Yorumu ekle
                await _commentRepository.AddAsync(comment);
                
                // Kullanıcı bilgilerini al (oluşturulan yorumda göstermek için)
                var commentWithUser = await _commentRepository.GetCommentWithUserAsync(comment.CommentId);
                
                if (commentWithUser == null || commentWithUser.User == null)
                {
                    return Json(new { 
                        success = true, 
                        message = "Yorumunuz eklendi, ancak görüntülemek için sayfayı yenilemeniz gerekebilir."
                    });
                }
                
               
                return Json(new { 
                    success = true, 
                    message = "Yorumunuz başarıyla eklendi.",
                    comment = new {
                        id = commentWithUser.CommentId,
                        content = commentWithUser.Content,
                        userName = commentWithUser.User.UserName ?? "Kullanıcı",
                        fullName = $"{commentWithUser.User.Name} {commentWithUser.User.Surname}",
                        profileImage = !string.IsNullOrEmpty(commentWithUser.User.ProfileImage) ? commentWithUser.User.ProfileImage : "userImage.jpg",
                        userId = commentWithUser.UserId,
                        createdAt = commentWithUser.CreatedAt.ToString("dd.MM.yyyy HH:mm")
                    }
                });
            }
            catch (Exception ex)
            {
                
                return Json(new { success = false, message = $"Yorum eklenirken bir hata oluştu: {ex.Message}" });
            }
        }
        
      
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return Json(new { success = false, message = "Yorum bulunamadı." });
            }

            // Kullanıcı yetkisi kontrolü (sadece kendi yorumunu veya admin silebilir)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            if (comment.UserId != userId && !isAdmin)
            {
                return Json(new { success = false, message = "Bu yorumu silme yetkiniz yok." });
            }

            try
            {
                await _commentRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Yorum başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Yorum silinirken bir hata oluştu: {ex.Message}" });
            }
        }
    }
} 