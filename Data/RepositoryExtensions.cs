using BlogProject.Data.Abstract;
using BlogProject.Data.Concrete.EfCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogProject.Data
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {//program.cs icin

            // Generic repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(EfGenericRepository<>));
            
            // Specific repositories
            services.AddScoped<ICategoryRepository, EfCategoryRepository>();
            services.AddScoped<IPostRepository, EfPostRepository>();
            services.AddScoped<ICommentRepository, EfCommentRepository>();
            services.AddScoped<IUserRepository, EfUserRepository>();
            
            return services;
        }
    }
} 