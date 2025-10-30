using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.ProductsDtos
{
    public class ProductUpdateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
