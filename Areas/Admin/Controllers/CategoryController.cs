using BlogProject.Data;
using BlogProject.Data.Abstract;
using BlogProject.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BlogProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryController> _logger;
        private readonly BlogContext _context;

        public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger, BlogContext context)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _context = context;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesWithPostCountAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategoriler listelenirken bir hata oluştu.");
                return View(Enumerable.Empty<Category>());
            }
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
        {
            try
            {
                if (string.IsNullOrEmpty(category.Name))
                {
                    ModelState.AddModelError("Name", "Kategori adı gereklidir.");
                    return View(category);
                }

                category.Url = GenerateSeoFriendlyUrl(category.Name);
                await _categoryRepository.AddAsync(category);
                TempData["SuccessMessage"] = "Kategori başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategori eklenirken bir hata oluştu. Name: {Name}, Description: {Description}", 
                    category.Name, category.Description);
                
                ModelState.AddModelError("", "Kategori eklenirken bir hata oluştu: " + ex.Message);
                return View(category);
            }
        }

       
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name,Description,Url")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Kategori güncelleniyor. ID: {CategoryId}, Name: {Name}, Description: {Description}",
                        category.CategoryId, category.Name, category.Description);
                    
                    // Kategori adı değiştiyse URL'i güncelle
                    var existingCategory = await _categoryRepository.GetByIdAsync(id);
                    if (existingCategory != null)
                    {
                        if (existingCategory.Name != category.Name)
                        {
                            category.Url = GenerateSeoFriendlyUrl(category.Name);
                            _logger.LogInformation("Kategori adı değiştirildi, yeni URL: {Url}", category.Url);
                        }

                      
                        _context.Entry(existingCategory).State = EntityState.Detached;
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Kategori başarıyla güncellendi.");
                    
                    TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await _categoryRepository.ExistsAsync(c => c.CategoryId == id))
                    {
                        _logger.LogWarning("Güncellenmeye çalışılan kategori bulunamadı. ID: {CategoryId}", id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Kategori güncellenirken eşzamanlılık hatası oluştu. ID: {CategoryId}", id);
                        ModelState.AddModelError("", "Kategori güncellenirken eşzamanlılık hatası oluştu: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kategori güncellenirken bir hata oluştu. ID: {CategoryId}", id);
                    ModelState.AddModelError("", "Kategori güncellenirken bir hata oluştu: " + ex.Message);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Kategori düzenlemede model doğrulama hatası. Hatalar: {Errors}", string.Join(", ", errors));
            }

            return View(category);
        }

        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetCategoryWithPostsAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.Description = category.Description;
            ViewBag.PostCount = category.Posts?.Count ?? 0;

            return View(category);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Kategoriyi ilişkili postlarla birlikte getirir
                var category = await _categoryRepository.GetCategoryWithPostsAsync(id);
                
                if (category == null)
                {
                    return NotFound();
                }

                // Kategoriye bağlı blog yazıları var mı kontrol eder
                if (category.Posts != null && category.Posts.Count > 0)
                {
                    _logger.LogWarning("Kategoriye bağlı {count} adet blog yazısı olduğu için silinemedi.", category.Posts.Count);
                    TempData["ErrorMessage"] = $"Bu kategoriye bağlı {category.Posts.Count} adet blog yazısı olduğu için silinemez. Önce bu yazıları başka bir kategoriye taşıyın veya silin.";
                    return RedirectToAction(nameof(Index));
                }

                // Kategori boşsa sil
                await _categoryRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Foreign key kısıtlaması nedeniyle kategori silinemedi. ID: {CategoryId}", id);
                TempData["ErrorMessage"] = "Bu kategori diğer verilerle ilişkili olduğu için silinemez.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategori silinirken bir hata oluştu. ID: {CategoryId}", id);
                TempData["ErrorMessage"] = "Kategori silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // URL oluşturma yardımcı metodu
        private string GenerateSeoFriendlyUrl(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

         
            text = text.ToLower()
                .Replace("ı", "i")
                .Replace("ğ", "g")
                .Replace("ü", "u")
                .Replace("ş", "s")
                .Replace("ç", "c")
                .Replace("ö", "o")
                .Replace(" ", "-")
                .Replace(".", "");

            // url için ayarlama
            text = Regex.Replace(text, @"[^a-z0-9\-]", "");
            text = Regex.Replace(text, @"-+", "-");
            text = text.Trim('-');

            return text;
        }
    }
} 