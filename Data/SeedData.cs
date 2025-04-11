using BlogProject.Data.Concrete.EfCore;
using BlogProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<BlogContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

         
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            // Rol ekleme
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Kategorileri ekle
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Teknoloji", Url = "teknoloji" },
                    new Category { Name = "Programlama", Url = "programlama" },
                    new Category { Name = "Web Geliştirme", Url = "web-gelistirme" },
                    new Category { Name = "Veritabanı", Url = "veritabani" },
                    new Category { Name = ".NET Core", Url = "net-core" }
                );
                await context.SaveChangesAsync();
            }
            else
            {
                // Mevcut kategorilerin Url değerlerini güncelle
                var categories = await context.Categories.ToListAsync();
                bool hasChanges = false;
                
                foreach (var category in categories)
                {
                    if (string.IsNullOrEmpty(category.Url))
                    {
                    
                        category.Url = category.Name.ToLower()
                            .Replace(" ", "-")
                            .Replace("ç", "c")
                            .Replace("ğ", "g")
                            .Replace("ı", "i")
                            .Replace("ö", "o")
                            .Replace("ş", "s")
                            .Replace("ü", "u")
                            .Replace(".", "");
                        
                        hasChanges = true;
                    }
                }
                
                if (hasChanges)
                {
                    await context.SaveChangesAsync();
                }
            }

            // Kullanıcıları ekle
            if (!userManager.Users.Any())
            {
                // Örnek admin 
                var adminUser = new User
                {
                    UserName = "admin@blogum.com",
                    Email = "admin@blogum.com",
                    EmailConfirmed = true,
                    Name = "Admin",
                    Surname = "User",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                var adminResult = await userManager.CreateAsync(adminUser, "Admin123!");
                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                // Normal kullanıcılar
                var user1 = new User
                {
                    UserName = "omersargin@deneme.com",
                    Email = "omersargin@deneme.com",
                    EmailConfirmed = true,
                    Bio = "Yazılım mühendisi",
                    ProfileImage = "p1.jpg",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                var result1 = await userManager.CreateAsync(user1, "User123*");
                if (result1.Succeeded)
                {
                    await userManager.AddToRoleAsync(user1, "User");
                }

                var user2 = new User
                {
                    UserName = "cerendurmus@deneme.com",
                    Email = "cerendurmus@deneme.com",
                    EmailConfirmed = true,
                    Bio = "Web tasarımcı",
                    ProfileImage = "p2.jpg",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                var result2 = await userManager.CreateAsync(user2, "User123*");
                if (result2.Succeeded)
                {
                    await userManager.AddToRoleAsync(user2, "User");
                }
            }

            // Blog yazıları ekle
            if (!context.Posts.Any())
            {
                // Kullanıcı ID'lerini al
                var admin = await userManager.FindByEmailAsync("admin@blogum.com");
                var user1 = await userManager.FindByEmailAsync("omersargin@deneme.com");
                var user2 = await userManager.FindByEmailAsync("cerendurmus@deneme.com");

                // Kategorileri al
                var categories = await context.Categories.ToListAsync();

                if (admin != null && user1 != null && user2 != null)
                {
                    // Blog yazıları ekle
                    context.Posts.AddRange(
                        new Post
                        {
                            Title = "ASP.NET Core ile MVC Uygulamaları",
                            Content = @"<p>ASP.NET Core MVC, modern web uygulamaları geliştirmek için kullanılan güçlü bir frameworktür.</p>
                                      <p>Bu yazıda, ASP.NET Core MVC'nin temel bileşenlerini ve nasıl kullanılacağını öğreneceksiniz.</p>
                                      <h3>Model-View-Controller (MVC) Nedir?</h3>
                                      <p>MVC, bir yazılım mimari desenidir ve uygulamanızı üç ana bileşene ayırır:</p>
                                      <ul>
                                        <li><strong>Model:</strong> Uygulamanızın veri ve iş mantığını temsil eder.</li>
                                        <li><strong>View:</strong> Kullanıcı arayüzünü temsil eder.</li>
                                        <li><strong>Controller:</strong> Kullanıcı etkileşimlerini işler ve modeli günceller.</li>
                                      </ul>",
                            Description = "ASP.NET Core MVC ile modern web uygulamaları geliştirmeyi öğrenin.",
                            Url = "aspnet-core-mvc-uygulamalari",
                            Image = "1.jpg",
                            PublishedOn = DateTime.Now.AddDays(-10),
                            IsActive = true,
                            UserId = admin.Id,
                            CategoryId = categories[0].CategoryId
                        },
                        new Post
                        {
                            Title = "Entity Framework Core ile Veritabanı İşlemleri",
                            Content = @"<p>Entity Framework Core, .NET uygulamalarında ORM (Object-Relational Mapping) için kullanılan bir kütüphanedir.</p>
                                      <p>Bu yazıda, Entity Framework Core kullanarak veritabanı işlemlerini nasıl yapacağınızı öğreneceksiniz.</p>
                                      <h3>Entity Framework Core Avantajları</h3>
                                      <ul>
                                        <li>Veritabanı işlemlerini nesne yönelimli bir şekilde yapabilirsiniz.</li>
                                        <li>LINQ sorgularını kullanarak veritabanı sorguları yazabilirsiniz.</li>
                                        <li>Migration sistemi ile veritabanı şemasını kolayca yönetebilirsiniz.</li>
                                      </ul>",
                            Description = "Entity Framework Core ile veritabanı işlemlerini kolayca yapın.",
                            Url = "entity-framework-core-veritabani-islemleri",
                            Image = "2.jpg",
                            PublishedOn = DateTime.Now.AddDays(-5),
                            IsActive = true,
                            UserId = user1.Id,
                            CategoryId = categories[3].CategoryId
                        },
                        new Post
                        {
                            Title = "Responsive Web Tasarımı İlkeleri",
                            Content = @"<p>Responsive web tasarımı, web sitelerinizin farklı cihazlarda doğru şekilde görüntülenmesini sağlar.</p>
                                      <p>Bu yazıda, responsive web tasarımının temel ilkelerini ve nasıl uygulanacağını öğreneceksiniz.</p>
                                      <h3>Responsive Tasarımın Temel İlkeleri</h3>
                                      <ul>
                                        <li><strong>Esnek Izgara Sistemi:</strong> Bootstrap gibi framework'ler kullanarak esnek ızgara sistemleri oluşturabilirsiniz.</li>
                                        <li><strong>Esnek Görüntüler:</strong> Görüntülerin farklı ekran boyutlarına uyum sağlaması için CSS kullanabilirsiniz.</li>
                                        <li><strong>Medya Sorguları:</strong> CSS medya sorguları ile farklı cihazlar için özel stiller tanımlayabilirsiniz.</li>
                                      </ul>",
                            Description = "Responsive web tasarımının temel ilkelerini öğrenin.",
                            Url = "responsive-web-tasarimi-ilkeleri",
                            Image = "3.jpg",
                            PublishedOn = DateTime.Now.AddDays(-3),
                            IsActive = true,
                            UserId = user2.Id,
                            CategoryId = categories[2].CategoryId
                        }
                    );
                    await context.SaveChangesAsync();

                    // Yorumları ekle
                    var posts = await context.Posts.ToListAsync();
                    if (posts.Any())
                    {
                        context.Comments.AddRange(
                            new Comment
                            {
                                Content = "Harika bir yazı, teşekkürler!",
                                PostId = posts[0].PostId,
                                UserId = user1.Id
                            },
                            new Comment
                            {
                                Content = "Bu konuda daha fazla bilgi paylaşabilir misiniz?",
                                PostId = posts[0].PostId,
                                UserId = user2.Id
                            },
                            new Comment
                            {
                                Content = "Entity Framework Code First yaklaşımını da anlatır mısınız?",
                                PostId = posts[1].PostId,
                                UserId = admin.Id
                            },
                            new Comment
                            {
                                Content = "Bootstrap 5 ile ilgili de bir yazı yazabilir misiniz?",
                                PostId = posts[2].PostId,
                                UserId = user1.Id
                            }
                        );
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
} 