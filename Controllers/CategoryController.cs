using Microsoft.AspNetCore.Mvc;
using BlogProject.Data.Abstract;
using BlogProject.Entities;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace BlogProject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ICategoryRepository categoryRepository,
            IPostRepository postRepository,
            ILogger<CategoryController> logger)
        {
            _categoryRepository = categoryRepository;
            _postRepository = postRepository;
            _logger = logger;
        }

  
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

        public async Task<IActionResult> Details(int id, int page = 1, int pageSize = 6)
        {
            var category = await _categoryRepository.GetCategoryWithPostsAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            // Kategoriye ait yazıları sayfalandırarak al
            var posts = await _postRepository.GetPostsByCategoryAsync(id, page, pageSize);
            ViewBag.Posts = posts;
            
            // Toplam yazı sayısını al
            var totalPosts = await _postRepository.GetPostCountByCategoryAsync(id);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalPosts / pageSize);
            ViewBag.PageSize = pageSize;

            return View(category);
        }

     
        [Route("Category/{url}")]
        public async Task<IActionResult> ByUrl(string url, int page = 1, int pageSize = 6)
        {
            var category = await _categoryRepository.GetCategoryByUrlAsync(url);
            if (category == null)
            {
                return NotFound();
            }

            // Kategoriye ait yazıları sayfalandırarak al
            var posts = await _postRepository.GetPostsByCategoryAsync(category.CategoryId, page, pageSize);
            ViewBag.Posts = posts;
            
            // Toplam yazı sayısını al
            var totalPosts = await _postRepository.GetPostCountByCategoryAsync(category.CategoryId);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalPosts / pageSize);
            ViewBag.PageSize = pageSize;
            
            return View("Details", category);
        }

        
        public async Task<IActionResult> List()
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesWithPostCountAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kategori listesi getirilirken bir hata oluştu: {Message}", ex.Message);
                return View(Enumerable.Empty<Category>());
            }
        }
    }
} 