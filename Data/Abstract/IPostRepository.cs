using BlogProject.Entities;
using System.Linq.Expressions;

namespace BlogProject.Data.Abstract
{
    public interface IPostRepository : IGenericRepository<Post>
    {
       //Blog postuna özel metot
        Task<Post> GetByUrlAsync(string url);
        
        // Sayfalama 
        Task<IEnumerable<Post>> GetPostsPaginatedAsync(int page, int pageSize);
        Task<int> GetTotalPostCountAsync();
        
        // İlişkili sorgular
        Task<Post> GetPostWithUserAsync(int postId);
        Task<Post> GetPostWithCommentsAsync(int postId);
        Task<Post> GetPostWithCategoryAsync(int postId);
        Task<Post> GetPostWithAllDetailsAsync(int postId);
        Task<Post> GetPostWithAllDetailsByUrlAsync(string url);
        
        // Admin paneli 
        Task<IEnumerable<Post>> GetAllPostsWithDetailsAsync();
        Task<Post> GetPostWithDetailsAsync(int postId);
        Task<IEnumerable<Post>> GetRecentPostsAsync(int count);
        
        // Filtreleme 
        Task<IEnumerable<Post>> GetPostsByCategoryAsync(int categoryId, int page, int pageSize);
        Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, int page, int pageSize);
        Task<IEnumerable<Post>> GetActivePostsAsync(int page, int pageSize);
        Task<IEnumerable<Post>> SearchPostsAsync(string searchTerm, int page, int pageSize);
        Task<int> GetPostCountByCategoryAsync(int categoryId);
        
        // Kullanıcının blog yazısı sayısını getir
        Task<int> GetPostCountByUserAsync(string userId);
    }
} 