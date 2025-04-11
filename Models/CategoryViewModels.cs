using System.ComponentModel.DataAnnotations;
using BlogProject.Entities;

namespace BlogProject.Models
{
    
    public class CategoryCreateEditViewModel
    {
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [StringLength(50, ErrorMessage = "Kategori adı en fazla {1} karakter olabilir")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; }
    }

    // Kategori listeleme için View Model
    public class CategoryListViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
    }

   
    public class CategoryDeleteViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int PostCount { get; set; }
    }
} 