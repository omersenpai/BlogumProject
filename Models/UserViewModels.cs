using BlogProject.Entities;
using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    // Kullanıcı listesi için ViewModel
    public class UserViewModel
    {
        public string Id { get; set; }
        
        [Display(Name = "Ad")]
        public string Name { get; set; }
        
        [Display(Name = "Soyad")]
        public string Surname { get; set; }
        
        [Display(Name = "E-posta")]
        public string Email { get; set; }
        
        [Display(Name = "Kayıt Tarihi")]
        public DateTime CreatedAt { get; set; }
        
        [Display(Name = "Hakkında")]
        public string Bio { get; set; }
        
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }
        
        [Display(Name = "Silinmiş")]
        public bool IsDeleted { get; set; }
        
        [Display(Name = "Roller")]
        public List<string> Roles { get; set; }
        
        [Display(Name = "Blog Yazısı Sayısı")]
        public int PostCount { get; set; }
        
        [Display(Name = "Yorum Sayısı")]
        public int CommentCount { get; set; }
        
        // Tam ad getter
        public string FullName => $"{Name} {Surname}";
    }

    // Kullanıcı düzenleme için ViewModel
    public class UserEditViewModel
    {
        public string Id { get; set; }
        
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [Display(Name = "Ad")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [Display(Name = "Soyad")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string Surname { get; set; }
        
        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
        
        [Display(Name = "Hakkında")]
        [StringLength(500, ErrorMessage = "Biyografi en fazla 500 karakter olabilir")]
        public string Bio { get; set; }
        
        [Display(Name = "Durum")]
        public bool IsActive { get; set; }
    }
} 