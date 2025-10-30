using Application.Dtos.OrdersItemDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.OrdersDto
{
    public class OrderCreateDto
    {
        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public List<OrderItemCreateDto> Items { get; set; }
    }
}
