using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BlogProject.Entities;
using BlogProject.Models;
using BlogProject.Models.ViewModels;
using BlogProject.Services.Abstract;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using BlogProject.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System;
using System.Threading.Tasks;
using BlogProject.Services;

namespace BlogProject.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly BlogContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            BlogContext context,
            ILogger<AccountController> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                // Kullanıcıyı bul
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                if (user != null)
                {
                    // Silinmiş kullanıcı kontrolü
                    if (user.IsDeleted)
                    {
                        ModelState.AddModelError(string.Empty, "Bu hesap silinmiş durumda. Lütfen yönetici ile iletişime geçin.");
                        return View(model);
                    }
                    
                    // Pasif kullanıcı kontrolü
                    if (!user.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "Bu hesap pasif durumda. Lütfen yönetici ile iletişime geçin.");
                        return View(model);
                    }
                }
                
                // Kullanıcı girişini dene
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                
                if (result.Succeeded)
                {
                    // Login başarılı - kullanıcı bilgilerini al
                    if (user != null)
                    {
                        // Son kez kontrol - eğer silindiyse oturum açmayı engelle
                        if (user.IsDeleted)
                        {
                            await _signInManager.SignOutAsync(); // Oturumu kapat
                            ModelState.AddModelError(string.Empty, "Bu hesap silinmiş durumda. Lütfen yönetici ile iletişime geçin.");
                            return View(model);
                        }

                        // Kolay erişim için kullanıcı adı claim'i oluştur
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Name)
                        };
                        
                        await _userManager.AddClaimsAsync(user, claims);
                    }
                    
                   
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
                
                if (result.IsLockedOut)
                {
                    return RedirectToAction(nameof(Lockout));
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Bu hesap aktif değil veya silinmiş.");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
                    return View(model);
                }
            }
            
            return View(model);
        }

        // Kullanıcı hesabı kilitli sayfası
        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }

     
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    ProfileImage = "userImage.jpg", 
                    Bio = string.Empty, 
                    IsActive = true, 
                    IsDeleted = false, 
                    CreatedAt = DateTime.Now 
                };
                
                // Kullanıcıyı veritabanına ekle
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    // Kullanıcıya User rolü ata
                    await _userManager.AddToRoleAsync(user, "User");
                    
                    // İsim için özel claim oluştur (Views'da göstermek için)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name)
                    };
                    
                    await _userManager.AddClaimsAsync(user, claims);
                    
                    // Otomatik giriş yap
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

   
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound();
            }
            
            var model = new ProfileViewModel
            {
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Bio = user.Bio,
                ProfileImage = user.ProfileImage
            };
            
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model, IFormFile? profileImage)
        {
            // ProfileImage alanı için hata varsa temizle (zorunlu alan değil)
            if (!ModelState.IsValid && ModelState.ContainsKey("ProfileImage"))
            {
                ModelState.Remove("ProfileImage");
            }
            
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation($"Profil güncellemesi başladı. UserId: {userId}");
                
                var user = await _userManager.FindByIdAsync(userId);
                
                if (user == null)
                {
                    _logger.LogWarning($"Kullanıcı bulunamadı. UserId: {userId}");
                    return NotFound();
                }
                
                try
                {
                    // Kullanıcı bilgilerini güncelle
                    user.Name = model.Name;
                    user.Surname = model.Surname;
                    user.Bio = model.Bio;
                    
                    // Profil resmi yükleme işlemi
                    if (profileImage != null && profileImage.Length > 0)
                    {
                        _logger.LogInformation($"Profil resmi yükleniyor. Boyut: {profileImage.Length} bytes");
                        
                        // Benzersiz dosya adı oluştur
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                        // Kaydedilecek yolu belirle
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);
                        
                        // img klasörünün var olduğunu kontrol et, yoksa oluştur
                        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        
                        // Dosyayı kaydet
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await profileImage.CopyToAsync(stream);
                        }
                        
                        // Yeni profil resmini ata
                        user.ProfileImage = fileName;
                    }
                    
                    // Kullanıcı bilgilerini veritabanında güncelle
                    var result = await _userManager.UpdateAsync(user);
                    
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Kullanıcı bilgileri güncellendi.");
                        
                        // Kullanıcı adı claim'ini de güncelle (navbar'da isim gösterimi için)
                        try
                        {
                            var claims = await _userManager.GetClaimsAsync(user);
                            var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                            
                            if (nameClaim != null)
                            {
                                // Eski claim'i kaldır
                                await _userManager.RemoveClaimAsync(user, nameClaim);
                            }
                            
                            // Yeni claim ekle
                            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.Name));
                        }
                        catch (Exception claimEx)
                        {
                            _logger.LogError($"Claim güncellemede hata: {claimEx.Message}");
                            // Claim hatası olsa bile devam et
                        }
                        
                        // Oturumu yenile (claim değişikliklerinin aktif olması için)
                        await _signInManager.RefreshSignInAsync(user);
                        
                        TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
                        return RedirectToAction(nameof(Profile));
                    }
                    
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"Profil güncellemede hata: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Profil güncellemede hata: {ex.Message}");
                    ModelState.AddModelError(string.Empty, $"Profil güncellenirken bir hata oluştu: {ex.Message}");
                    TempData["ErrorMessage"] = "Profil güncellenirken bir hata oluştu.";
                }
            }
            
            // Hata durumunda veya ModelState geçersiz olduğunda, modeli view'a geri döndür
            return View(model);
        }


        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound();
            }
            
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            
            if (result.Succeeded)
            {
                // Şifre değiştiğinde oturumu yenile
                await _signInManager.RefreshSignInAsync(user);
                
                TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
                
                return RedirectToAction(nameof(Profile));
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Güvenlik için kullanıcı bulunamasa bile aynı mesajı göster
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { email = user.Email, code = code },
                    protocol: Request.Scheme);

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Şifre Sıfırlama",
                    $"Lütfen şifrenizi sıfırlamak için <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>buraya tıklayın</a>.");

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("Şifre sıfırlama için bir kod gereklidir.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Güvenlik için kullanıcı bulunamasa bile başarılı sayfasına yönlendir
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
} 