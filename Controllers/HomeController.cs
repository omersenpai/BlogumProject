using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BlogProject.Models;
using BlogProject.Data.Abstract;
using BlogProject.Entities;

namespace BlogProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPostRepository _postRepository;
    private readonly ICategoryRepository _categoryRepository;

    public HomeController(ILogger<HomeController> logger,IPostRepository postRepository,ICategoryRepository categoryRepository)
    {
        _logger = logger;
        _postRepository = postRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 9)
    {
        var posts = await _postRepository.GetActivePostsAsync(page, pageSize);
        var totalPosts = await _postRepository.GetTotalPostCountAsync();
        
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalPosts / pageSize);
        ViewBag.PageSize = pageSize;
        
        // Kategorileri sidebar'da göstermek için
        ViewBag.Categories = await _categoryRepository.GetCategoriesWithPostCountAsync();
        
        return View(posts);
    }

    public async Task<IActionResult> Search(string query, int page = 1, int pageSize = 9)
    {
        if (string.IsNullOrWhiteSpace(query))
            return RedirectToAction("Index");
            
        var posts = await _postRepository.SearchPostsAsync(query, page, pageSize);
        
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.SearchQuery = query;
        ViewBag.Categories = await _categoryRepository.GetCategoriesWithPostCountAsync();
        
        return View("Index", posts);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
