using BlogProject.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlogProject.Data
{
    public class CustomSignInManager : SignInManager<User>
    {
        public CustomSignInManager(
            UserManager<User> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<User> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                // Kullanıcı silinmiş ise giriş yapamasın 
                if (user.IsDeleted)
                {
                    return SignInResult.NotAllowed;
                }
                
                // Kullanıcı pasif durumda ise giriş yapamasın
                if (!user.IsActive)
                {
                    return SignInResult.NotAllowed;
                }
            }
            
            var result = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
            
            // bir kere daha kontrol edelim giris sonrası
            if (result.Succeeded && user != null && (user.IsDeleted || !user.IsActive))
            {
                // Kullanıcı silindi veya deaktive edildiyse oturumu sonlandır
                await SignOutAsync();
                return SignInResult.NotAllowed;
            }

            return result;
        }
    }
} 