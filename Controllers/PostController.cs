using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogProject.Data.Abstract;
using BlogProject.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace BlogProject.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public PostController(
            IPostRepository postRepository,
            ICategoryRepository categoryRepository,
            IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        // Tüm blog yazılarını listeler
        public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
        {
            var posts = await _postRepository.GetPostsPaginatedAsync(page, pageSize);
            var totalPosts = await _postRepository.GetTotalPostCountAsync();
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalPosts / pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Categories = await _categoryRepository.GetAllAsync();
            
            return View(posts);
        }

        // Blog yazısının detayları
        public async Task<IActionResult> Details(int id)
        {
            var post = await _postRepository.GetPostWithAllDetailsAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Tüm kategorileri post sayılarıyla beraber getir
            ViewBag.Categories = await _categoryRepository.GetCategoriesWithPostCountAsync();
            
            return View(post);
        }

        // URL ile blog yazısı detayları
        public async Task<IActionResult> ByUrl(string url)
        {
            // Post nesnesini tüm ilişkili verileriyle birlikte yükle
            var post = await _postRepository.GetPostWithAllDetailsByUrlAsync(url);
            if (post == null)
            {
                return NotFound();
            }

            // Tüm kategorileri post sayılarıyla beraber getir
            ViewBag.Categories = await _categoryRepository.GetCategoriesWithPostCountAsync();
            return View("Details", post);
        }

 
        public async Task<IActionResult> Category(int id, int page = 1, int pageSize = 6)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var posts = await _postRepository.GetPostsByCategoryAsync(id, page, pageSize);
            var totalPosts = await _postRepository.GetPostCountByCategoryAsync(id);
            
            ViewBag.Category = category;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalPosts / pageSize);
            ViewBag.Categories = await _categoryRepository.GetCategoriesWithPostCountAsync();
            
            return View(posts);
        }
        
        // Kategori bazlı blog yazıları listesi (URL ile)
        [Route("kategori/{url}")]
        public async Task<IActionResult> CategoryByUrl(string url, int page = 1, int pageSize = 6)
        {
            var category = await _categoryRepository.GetCategoryByUrlAsync(url);
            if (category == null)
            {
                return NotFound();
            }

            var posts = await _postRepository.GetPostsByCategoryAsync(category.CategoryId, page, pageSize);
            var totalPosts = await _postRepository.GetPostCountByCategoryAsync(category.CategoryId);
            
            ViewBag.Category = category;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalPosts / pageSize);
            ViewBag.Categories = await _categoryRepository.GetCategoriesWithPostCountAsync();
            
            return View("Category", posts);
        }

        // Kullanıcının yazdığı blog yazılarını listeler
        [Route("Post/UserPosts/{id}")]
        [Route("Yazar/{name}/{id}")]
        public async Task<IActionResult> UserPosts(string id, string name = null, int page = 1, int pageSize = 6)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var posts = await _postRepository.GetPostsByUserAsync(id, page, pageSize);
            
            ViewBag.User = user;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Categories = await _categoryRepository.GetAllAsync();
            
            return View(posts);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryRepository.GetAllAsync();
            return View(new Post());
        }

        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, IFormFile? imageFile)
        {
            try
            {
                // Validasyon hatalarını kontrol ediyoruz
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation Error: {error}");
                    }
                    
                    TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları doldurun.";
                    ViewBag.Categories = await _categoryRepository.GetAllAsync();
                    return View(post);
                }

              
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
               
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Kullanıcı ID'si null veya boş. Kullanıcı giriş yapmamış olabilir.");
                    ModelState.AddModelError("", "Kullanıcı bilgisi bulunamadı. Lütfen tekrar giriş yapın.");
                    TempData["ErrorMessage"] = "Blog yazısı eklemek için önce giriş yapmalısınız.";
                    return RedirectToAction("Login", "Account");
                }
                
                post.UserId = userId;
                post.PublishedOn = DateTime.Now;
                post.IsActive = true;

                // URL olusturma 
                string slugifiedTitle = post.Title.ToLower()
                    .Replace(" ", "-")
                    // Türkçe karakterleri dönüştür
                    .Replace("ı", "i")
                    .Replace("ğ", "g")
                    .Replace("ü", "u")
                    .Replace("ş", "s")
                    .Replace("ç", "c")
                    .Replace("ö", "o")
                    // Özel karakterleri kaldır
                    .Replace("?", "")
                    .Replace("!", "")
                    .Replace(".", "")
                    .Replace(",", "")
                    .Replace(":", "")
                    .Replace(";", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("\"", "")
                    .Replace("'", "")
                    .Replace("`", "")
                    .Replace("&", "")
                    .Replace("%", "")
                    .Replace("+", "")
                    .Replace("=", "")
                    .Replace("/", "")
                    .Replace("\\", "")
                    .Replace("|", "")
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace("@", "")
                    .Replace("#", "")
                    .Replace("$", "")
                    .Replace("^", "")
                    .Replace("*", "")
                    .Replace("~", "");
                
               //url icin
                while (slugifiedTitle.Contains("--"))
                {
                    slugifiedTitle = slugifiedTitle.Replace("--", "-");
                }
                slugifiedTitle = slugifiedTitle.Trim('-');
                post.Url = slugifiedTitle;
                
                // EF Core bunları otomatik olarak dolduracak
                post.User = null;
                post.Category = null;
                
         
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                   
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);
                        
                     
                        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        
                      
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        
                   
                        post.Image = fileName;
                        Console.WriteLine($"Resim başarıyla yüklendi: {fileName}");
                    }
                    else
                    {
                        // Varsayılan resim
                        post.Image = "1.jpg";
                        Console.WriteLine("Varsayılan resim kullanıldı: 1.jpg");
                    }
                }
                catch (Exception imgEx)
                {
                    // Resim yükleme hatası olsa bile devam et, varsayılan resim kullan
                    Console.WriteLine($"Resim yükleme hatası: {imgEx.Message}");
                    post.Image = "1.jpg";
                }

                // ModelState'i temizleyerek olası valid olmayan property'leri yoksay
                ModelState.Clear();
                
             
                Console.WriteLine($"Blog yazısı kaydediliyor: {post.Title}");
                await _postRepository.AddAsync(post);
                
                TempData["SuccessMessage"] = "Blog yazısı başarıyla eklendi.";
                Console.WriteLine("Blog yazısı başarıyla eklendi.");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Blog yazısı eklenirken hata: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                ModelState.AddModelError("", $"Blog yazısı eklenirken bir hata oluştu: {ex.Message}");
                TempData["ErrorMessage"] = "Blog yazısı eklenirken bir hata oluştu. Lütfen tekrar deneyin.";
            }
            
            
            ViewBag.Categories = await _categoryRepository.GetAllAsync();
            return View(post);
        }

      
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            
            // Kullanıcı yetkisi kontrolü
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            if (post.UserId != userId && !isAdmin)
            {
                return Forbid();
            }
            
            ViewBag.Categories = await _categoryRepository.GetAllAsync();
            return View(post);
        }

        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post, IFormFile? imageFile)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            
       
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            if (post.UserId != userId && !isAdmin)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Var olan blog yazısını al
                    var existingPost = await _postRepository.GetByIdAsync(id);
                    
                    // Değiştirilmemesi gereken alanları koru
                    post.UserId = existingPost.UserId;
                    post.PublishedOn = existingPost.PublishedOn;
                    
                    // Başlık değiştiyse URL'yi güncelle
                    if (existingPost.Title != post.Title)
                    {
                        // URL oluştur 
                        string slugifiedTitle = post.Title.ToLower()
                            .Replace(" ", "-")
                            // Türkçe karakterleri dönüştür
                            .Replace("ı", "i")
                            .Replace("ğ", "g")
                            .Replace("ü", "u")
                            .Replace("ş", "s")
                            .Replace("ç", "c")
                            .Replace("ö", "o")
                            // Özel karakterleri kaldır
                            .Replace("?", "")
                            .Replace("!", "")
                            .Replace(".", "")
                            .Replace(",", "")
                            .Replace(":", "")
                            .Replace(";", "")
                            .Replace("(", "")
                            .Replace(")", "")
                            .Replace("[", "")
                            .Replace("]", "")
                            .Replace("\"", "")
                            .Replace("'", "")
                            .Replace("`", "")
                            .Replace("&", "")
                            .Replace("%", "")
                            .Replace("+", "")
                            .Replace("=", "")
                            .Replace("/", "")
                            .Replace("\\", "")
                            .Replace("|", "")
                            .Replace("<", "")
                            .Replace(">", "")
                            .Replace("@", "")
                            .Replace("#", "")
                            .Replace("$", "")
                            .Replace("^", "")
                            .Replace("*", "")
                            .Replace("~", "");
                        
                        //url icin
                        while (slugifiedTitle.Contains("--"))
                        {
                            slugifiedTitle = slugifiedTitle.Replace("--", "-");
                        }
                        
                  
                        slugifiedTitle = slugifiedTitle.Trim('-');
                        
                        post.Url = slugifiedTitle;
                    }
                    else
                    {
              
                        post.Url = existingPost.Url;
                    }
                    
                    // Eğer yeni resim yüklendiyse güncelle
                    if (imageFile != null && imageFile.Length > 0)
                    {
                     
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                 
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);
                        
                    
                        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        
                        // Dosyayı kaydet
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        
                   
                        post.Image = fileName;
                    }
                    else
                    {
                  
                        post.Image = existingPost.Image;
                    }
                    
                    await _postRepository.UpdateAsync(post);
                    TempData["SuccessMessage"] = "Blog yazısı başarıyla güncellendi.";
                    return RedirectToAction("ByUrl", new { url = post.Url });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Blog yazısı güncellenirken bir hata oluştu: {ex.Message}");
                    TempData["ErrorMessage"] = "Blog yazısı güncellenirken bir hata oluştu. Lütfen tekrar deneyin.";
                }
            }
            
            ViewBag.Categories = await _categoryRepository.GetAllAsync();
            return View(post);
        }

   
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            
            // Kullanıcı yetkisi kontrolü
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            if (post.UserId != userId && !isAdmin)
            {
                return Forbid();
            }
            
            return View(post);
        }

    
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            
            // Kullanıcı yetkisi kontrolü
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            if (post.UserId != userId && !isAdmin)
            {
                return Forbid();
            }
            
            await _postRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index), "Home");
        }
    }
} 