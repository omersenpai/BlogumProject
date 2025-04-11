using BlogProject.Data.Abstract;
using BlogProject.Entities;
using BlogProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IPostRepository postRepository,
            ICategoryRepository categoryRepository,
            ICommentRepository commentRepository,
            UserManager<User> userManager,
            ILogger<DashboardController> logger)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Dashboard için
                var dashboardViewModel = new DashboardViewModel
                {
                    TotalPosts = await _postRepository.GetTotalPostCountAsync(),
                    TotalCategories = await _categoryRepository.CountAsync(),
                    TotalComments = await _commentRepository.CountAsync(),
                    TotalUsers = _userManager.Users.Count(),
                    RecentPosts = await _postRepository.GetRecentPostsAsync(5),
                    RecentComments = await _commentRepository.GetRecentCommentsAsync(5)
                };

                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard bilgileri alınırken bir hata oluştu.");
                return View(new DashboardViewModel());
            }
        }
    }
} 