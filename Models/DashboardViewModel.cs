using BlogProject.Entities;

namespace BlogProject.Models
{
    public class DashboardViewModel
    {
        public int TotalPosts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalComments { get; set; }
        public int TotalUsers { get; set; }
        public IEnumerable<Post> RecentPosts { get; set; } = Enumerable.Empty<Post>();
        public IEnumerable<Comment> RecentComments { get; set; } = Enumerable.Empty<Comment>();
    }
} 