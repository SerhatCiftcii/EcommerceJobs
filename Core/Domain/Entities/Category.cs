

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSolution.Core.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation property: Bu kategorideki ürünler
        public ICollection<Product> Products { get; set; }
    }
}