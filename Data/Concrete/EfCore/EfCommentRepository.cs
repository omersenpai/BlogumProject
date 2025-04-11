using BlogProject.Data.Abstract;
using BlogProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Data.Concrete.EfCore
{
    public class EfCommentRepository : EfGenericRepository<Comment>, ICommentRepository
    {
        public EfCommentRepository(BlogContext context) : base(context)
        {
        }

        public async Task<Comment> GetCommentWithUserAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);
        }

        public async Task<Comment> GetCommentWithPostAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.CommentId == commentId);
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostAsync(int postId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CommentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId)
        {
            return await _context.Comments
                .Include(c => c.Post)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CommentId)
                .ToListAsync();
        }

        public async Task<int> GetCommentCountByPostAsync(int postId)
        {
            return await _context.Comments
                .CountAsync(c => c.PostId == postId);
        }
        
        public async Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetCommentCountByUserAsync(string userId)
        {
            return await _context.Comments
                .Where(c => c.UserId == userId)
                .CountAsync();
        }
    }
} 