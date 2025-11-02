

using Application.Dtos.OrdersDto;
using Application.Dtos.OrdersItemDtos;

using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using ECommerceSolution.Core.Domain.Entities;
using ECommerceSolution.Infrastructure.Persistence.Repositories;
using Hangfire;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGenericRepository<Product> _productRepository; // Stok ve fiyat için
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository; // Kullanıcı bilgileri için

        public OrderService(
            IOrderRepository orderRepository,
            IGenericRepository<Product> productRepository,
            IUnitOfWork unitOfWork,
            IUserRepository userRepository) // constructor parametre olarak ekledik
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<(bool Success, string Message, OrderDto Order)> CreateOrderAsync(int userId, OrderCreateDto createDto)
        {
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            // 1. Stok ve Fiyat Kontrolü
            foreach (var item in createDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                if (product == null || product.StockQuantity < item.Quantity)
                {
                    return (false, $"Ürün bulunamadı veya '{product?.Name ?? "Bilinmeyen Ürün"}' için stok yetersiz. İstek: {item.Quantity}, Mevcut: {product?.StockQuantity ?? 0}", null);
                }

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPriceAtPurchase = product.Price
                };
                orderItems.Add(orderItem);

                totalAmount += orderItem.UnitPriceAtPurchase * item.Quantity;

                // Stoktan düş
                product.StockQuantity -= item.Quantity;
                _productRepository.Update(product);
            }

            // 2. Sipariş oluştur
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = createDto.ShippingAddress,
                TotalAmount = totalAmount,
                Status = "Pending",
                PaymentCompleted = false,
                OrderItems = orderItems
            };

            // 3. Veritabanına kaydet
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            var orderDto = MapOrderToOrderDto(order);

            
            // 4. Hangfire Job (Null-safe)
          
            // Kullanıcı email'ini repository üzerinden alıyoruz
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null && !string.IsNullOrWhiteSpace(user.Email))
            {
                BackgroundJob.Schedule<IEmailService>(
                    emailService => emailService.SendOrderConfirmationEmailAsync(
                        user.Email,
                        order.Id
                    ),
                    TimeSpan.FromMinutes(1) // 1 dakika sonra gönder
                );
            }

            return (true, "Sipariş başarıyla oluşturuldu.", orderDto);
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersWithDetailsByUserIdAsync(userId);
            return orders.Select(MapOrderToOrderDto).ToList();
        }

        public async Task<OrderDto> GetOrderDetailsAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);

            // Siparişin varlığı ve kullanıcının bu siparişe sahip olup olmadığı kontrolü
            if (order == null || order.UserId != userId)
            {
                return null;
            }

            return MapOrderToOrderDto(order);
        }

        // YÖNETİCİ METOTLARI
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();
            return orders.Select(MapOrderToOrderDto).ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            // Güvenlik: Sadece izin verilen statü değişimleri yapılabilir
            
            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            _orderRepository.Update(order);
            var affectedRows = await _unitOfWork.SaveChangesAsync();
            return affectedRows > 0;
        }


        // --- DTO Manuel Dönüşüm Metodu ---
        private OrderDto MapOrderToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                PaymentCompleted = order.PaymentCompleted,
                // OrderItem'ları da DTO'ya dönüştür
                Items = order.OrderItems?.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    // Eğer navigation property yüklendi ise Product Name'i al
                    ProductName = oi.Product?.Name ?? "Bilinmiyor",
                    Quantity = oi.Quantity,
                    UnitPriceAtPurchase = oi.UnitPriceAtPurchase
                }).ToList() ?? new List<OrderItemDto>()
            };
        }
    }
}