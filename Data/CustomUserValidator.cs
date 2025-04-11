using BlogProject.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlogProject.Data
{
    public class CustomUserValidator : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            // Kullanıcı pasif ise hata döndür
            if (!user.IsActive)
            {
                return Task.FromResult(IdentityResult.Failed(
                    new IdentityError { Code = "UserIsNotActive", Description = "Bu hesap pasif durumda. Lütfen yönetici ile iletişime geçin." }));
            }

            // Kullanıcı silinmiş ise hata döndür
            if (user.IsDeleted)
            {
                return Task.FromResult(IdentityResult.Failed(
                    new IdentityError { Code = "UserIsDeleted", Description = "Bu hesap silinmiş durumda. Lütfen yönetici ile iletişime geçin." }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
} 