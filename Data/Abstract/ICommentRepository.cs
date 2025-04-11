using BlogProject.Entities;
using System.Linq.Expressions;

namespace BlogProject.Data.Abstract
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        // İlişkili sorgular
        Task<Comment> GetCommentWithUserAsync(int commentId);
        Task<Comment> GetCommentWithPostAsync(int commentId);
        
        // Filtreleme işlemleri
        Task<IEnumerable<Comment>> GetCommentsByPostAsync(int postId);
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId);
        Task<int> GetCommentCountByPostAsync(int postId);
        
        // Dashboard için
        Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count);

        // Kullanıcının yorum sayısını getir
        Task<int> GetCommentCountByUserAsync(string userId);
    }
} 