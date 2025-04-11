using BlogProject.Data;
using BlogProject.Data.Abstract;
using BlogProject.Entities;
using BlogProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlogProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly BlogContext _context;

        public UserController(
            UserManager<User> userManager,
            IPostRepository postRepository,
            ICommentRepository commentRepository,
            BlogContext context)
        {
            _userManager = userManager;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _context = context;
        }

        // Kullanıcı listesi
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var postCount = await _postRepository.GetPostCountByUserAsync(user.Id);
                var commentCount = await _commentRepository.GetCommentCountByUserAsync(user.Id);

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive,
                    IsDeleted = user.IsDeleted,
                    Roles = roles.ToList(),
                    PostCount = postCount,
                    CommentCount = commentCount
                });
            }

            return View(userViewModels);
        }

        
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var postCount = await _postRepository.GetPostCountByUserAsync(user.Id);
            var commentCount = await _commentRepository.GetCommentCountByUserAsync(user.Id);

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Bio = user.Bio,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                Roles = roles.ToList(),
                PostCount = postCount,
                CommentCount = commentCount
            };

            return View(viewModel);
        }

        // Kullanıcı düzenleme get
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserEditViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Bio = user.Bio,
                IsActive = user.IsActive
            };

            return View(viewModel);
        }

        // Kullanıcı düzenleme post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.Bio = model.Bio;
            user.IsActive = model.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // Kullanıcı aktifleştirme veya deaktif etme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = user.IsActive ? "Kullanıcı aktifleştirildi." : "Kullanıcı deaktif edildi.";
            return RedirectToAction(nameof(Index));
        }

        // Şifre sıfırlama sayfası
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel
            {
                UserId = user.Id,
                Email = user.Email
            };

            return View(model);
        }

        // Şifre sıfırlama post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            TempData["SuccessMessage"] = "Kullanıcının şifresi başarıyla sıfırlandı.";
            return RedirectToAction(nameof(Index));
        }
        
        // Kullanıcı silme onay sayfası
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            return View(user);
        }
        
        // Kullanıcı icin soft delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "user-delete-logs.txt");
            System.IO.File.AppendAllText(logFilePath, $"### Kullanıcı Silme - {DateTime.Now} - ID: {id} ###\n");
            
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                System.IO.File.AppendAllText(logFilePath, "Kullanıcı bulunamadı.\n");
                return NotFound();
            }
            
            System.IO.File.AppendAllText(logFilePath, $"Silme öncesi kullanıcı durumu: IsDeleted={user.IsDeleted}, IsActive={user.IsActive}\n");
            
            try
            {
               
                user.IsDeleted = true;
                
                System.IO.File.AppendAllText(logFilePath, $"Değişiklik sonrası kullanıcı durumu: IsDeleted={user.IsDeleted}, IsActive={user.IsActive}\n");
                
                // Önce Identity ile güncellemeyi dene,sistemi yormamak acısından
                var result = await _userManager.UpdateAsync(user);
                System.IO.File.AppendAllText(logFilePath, $"UserManager.UpdateAsync sonucu: Başarılı={result.Succeeded}\n");
                
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        System.IO.File.AppendAllText(logFilePath, $"Hata: {error.Code} - {error.Description}\n");
                    }
                    
                    // UserManager başarısız olduysa, doğrudan DbContext ile deneyelim
                    System.IO.File.AppendAllText(logFilePath, "UserManager başarısız oldu, doğrudan context ile güncelleme deneniyor...\n");
                    
                    _context.Entry(user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    
                    System.IO.File.AppendAllText(logFilePath, "DbContext ile güncelleme tamamlandı.\n");
                }
                
                // Değişikliklerin veritabanına yansıdığını kontrol etmek için tekrar kullanıcıyı çekme
                var updatedUser = await _userManager.FindByIdAsync(id);
                System.IO.File.AppendAllText(logFilePath, $"Güncelleme sonrası DB'den alınan kullanıcı durumu: IsDeleted={updatedUser.IsDeleted}, IsActive={updatedUser.IsActive}\n");
                
                TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logFilePath, $"HATA: {ex.Message}\n{ex.StackTrace}\n");
                if (ex.InnerException != null)
                {
                    System.IO.File.AppendAllText(logFilePath, $"Inner Exception: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}\n");
                }
                
                TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
} 