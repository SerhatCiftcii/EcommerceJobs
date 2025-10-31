

using System.ComponentModel.DataAnnotations;

namespace ECommerceSolution.Core.Application.DTOs
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Kategori adı 100 karakteri geçemez.")]
        public string Name { get; set; }
    }
}