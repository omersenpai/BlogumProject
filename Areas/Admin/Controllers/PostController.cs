using BlogProject.Data.Abstract;
using BlogProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<PostController> _logger;

        public PostController(
            IPostRepository postRepository,
            ICategoryRepository categoryRepository,
            ILogger<PostController> logger)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

      
        public async Task<IActionResult> Index()
        {
            try
            {
                var posts = await _postRepository.GetAllPostsWithDetailsAsync();
                return View(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blog yazıları listelenirken bir hata oluştu.");
                return View(Enumerable.Empty<Post>());
            }
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var post = await _postRepository.GetPostWithDetailsAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", post.CategoryId);

            return View(post);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mevcut postu getir
                    var existingPost = await _postRepository.GetByIdAsync(id);
                    if (existingPost == null)
                    {
                        return NotFound();
                    }

                    // Sadece değiştirilebilir alanları güncelle
                    existingPost.Title = post.Title;
                    existingPost.Content = post.Content;
                    existingPost.Description = post.Description;
                    existingPost.CategoryId = post.CategoryId;
                    existingPost.IsActive = post.IsActive;
                    
                    // Postu güncelle
                    await _postRepository.UpdateAsync(existingPost);
                    TempData["SuccessMessage"] = "Blog yazısı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)//hata aldıgımız icin log actık
                {
                    _logger.LogError(ex, "Blog yazısı güncellenirken eşzamanlılık hatası oluştu. ID: {PostId}", id);
                    ModelState.AddModelError("", "Blog yazısı güncellenirken bir hata oluştu. Başka bir kullanıcı bu yazıyı düzenlemiş olabilir.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Blog yazısı güncellenirken bir hata oluştu. ID: {PostId}", id);
                    ModelState.AddModelError("", "Blog yazısı güncellenirken bir hata oluştu.");
                }
            }

            // Hata durumunda kategorileri tekrar yükle
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", post.CategoryId);
            
            return View(post);
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postRepository.GetPostWithDetailsAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

    
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var post = await _postRepository.GetByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }

                await _postRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Blog yazısı başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blog yazısı silinirken bir hata oluştu. ID: {PostId}", id);
                TempData["ErrorMessage"] = "Blog yazısı silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

       
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var post = await _postRepository.GetByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }

                // Blog yazısının durumunu değiştir
                post.IsActive = !post.IsActive;
                await _postRepository.UpdateAsync(post);

                string statusMessage = post.IsActive ? "aktifleştirildi" : "devre dışı bırakıldı";
                TempData["SuccessMessage"] = $"Blog yazısı başarıyla {statusMessage}.";
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blog yazısı durumu değiştirilirken bir hata oluştu. ID: {PostId}", id);
                TempData["ErrorMessage"] = "Blog yazısı durumu değiştirilirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 