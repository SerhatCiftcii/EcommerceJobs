

using System.ComponentModel.DataAnnotations;

namespace ECommerceSolution.Core.Application.DTOs
{
    public class CartItemManipulationDto
    {
        [Required(ErrorMessage = "Ürün ID'si zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Ürün ID'si geçerli olmalıdır.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Miktar zorunludur.")]
        [Range(1, 100, ErrorMessage = "Miktar 1 ile 100 arasında olmalıdır.")]
        public int Quantity { get; set; }
    }
}