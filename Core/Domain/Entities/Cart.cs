

using System;
using System.Collections.Generic;

namespace ECommerceSolution.Core.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; } 

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public DateTime? UpdatedAt { get; set; } 

        //  Sepetin sahibi olan kullanıcı
        public User User { get; set; }

        //  Sepetteki ürünlerin listesi (CartItems)
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}