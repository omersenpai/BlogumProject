using BlogProject.Data.Abstract;
using BlogProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Data.Concrete.EfCore
{
    public class EfPostRepository : EfGenericRepository<Post>, IPostRepository
    {
        public EfPostRepository(BlogContext context) : base(context)
        {
        }

        public override async Task AddAsync(Post entity)
        {
            // Navigation property'lerin null olması durumunda hata vermesini engelle
            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync();
        }

        public override async Task UpdateAsync(Post entity)
        {
            // Önce varolan entity'yi takipten çıkar
            var local = _context.Set<Post>()
                .Local
                .FirstOrDefault(entry => entry.PostId.Equals(entity.PostId));

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }
            
            // Yeni entity'yi ekle ve kaydet
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Post> GetByUrlAsync(string url)
        {
            return await _context.Posts
                .FirstOrDefaultAsync(p => p.Url == url);
        }

        public async Task<IEnumerable<Post>> GetPostsPaginatedAsync(int page, int pageSize)
        {
            return await _context.Posts
                .OrderByDescending(p => p.PublishedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalPostCountAsync()
        {
            return await _context.Posts.CountAsync();
        }

        public async Task<Post> GetPostWithUserAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<Post> GetPostWithCommentsAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<Post> GetPostWithCategoryAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<Post> GetPostWithAllDetailsAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<Post> GetPostWithAllDetailsByUrlAsync(string url)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Url == url);
        }
        
        // Admin paneli için metotlar
        public async Task<IEnumerable<Post>> GetAllPostsWithDetailsAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.PublishedOn)
                .ToListAsync();
        }

        public async Task<Post> GetPostWithDetailsAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<IEnumerable<Post>> GetRecentPostsAsync(int count)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .OrderByDescending(p => p.PublishedOn)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByCategoryAsync(int categoryId, int page, int pageSize)
        {
            return await _context.Posts
                .Where(p => p.CategoryId == categoryId)
                .OrderByDescending(p => p.PublishedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, int page, int pageSize)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PublishedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetActivePostsAsync(int page, int pageSize)
        {
            return await _context.Posts
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.PublishedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> SearchPostsAsync(string searchTerm, int page, int pageSize)
        {
            return await _context.Posts
                .Where(p => p.Title.Contains(searchTerm) || 
                           p.Content.Contains(searchTerm) || 
                           p.Description.Contains(searchTerm))
                .OrderByDescending(p => p.PublishedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetPostCountByCategoryAsync(int categoryId)
        {
            return await _context.Posts
                .Where(p => p.CategoryId == categoryId)
                .CountAsync();
        }

        public async Task<int> GetPostCountByUserAsync(string userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .CountAsync();
        }
    }
} 