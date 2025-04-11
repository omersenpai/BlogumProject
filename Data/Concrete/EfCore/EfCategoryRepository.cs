using BlogProject.Data.Abstract;
using BlogProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Data.Concrete.EfCore
{
    public class EfCategoryRepository : EfGenericRepository<Category>, ICategoryRepository
    {
        public EfCategoryRepository(BlogContext context) : base(context)
        {
        }

        public async Task<Category> GetCategoryWithPostsAsync(int categoryId)
        {
            return await _context.Categories
                .Include(c => c.Posts)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithPostCountAsync()
        {
            // Kategorileri ve ilişkili postları yükle
            var categories = await _context.Categories
                .Include(c => c.Posts.Where(p => p.IsActive)) // Sadece aktif postları dahil et
                .ToListAsync();

            
            foreach (var category in categories)
            {
                
                if (category.Posts == null)
                {
                    category.Posts = new List<Post>();
                }
            }

            return categories;
        }
        
        public async Task<Category> GetCategoryByUrlAsync(string url)
        {
            return await _context.Categories
                .Include(c => c.Posts.Where(p => p.IsActive))
                .FirstOrDefaultAsync(c => c.Url == url);
        }
    }
}