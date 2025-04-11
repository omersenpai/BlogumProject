using BlogProject.Entities;
using System.Linq.Expressions;

namespace BlogProject.Data.Abstract
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // İlişkili sorgular
        Task<Category> GetCategoryWithPostsAsync(int categoryId);
        Task<IEnumerable<Category>> GetCategoriesWithPostCountAsync();
        
        // URL ile kategori bulma
        Task<Category> GetCategoryByUrlAsync(string url);
    }
} 