using BlogProject.Entities;
using System.Linq.Expressions;

namespace BlogProject.Data.Abstract
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // User'a özel metodlar
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
        Task<bool> IsInRoleAsync(string userId, string roleName);
        
        // İlişkili sorgular
        Task<User> GetUserWithPostsAsync(string userId);
        Task<User> GetUserWithCommentsAsync(string userId);
    }
} 