// ECommerceSolution.Infrastructure/Services/OrderService.cs

using Application.Dtos.OrdersDto;
using Application.Dtos.OrdersItemDtos;

using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using ECommerceSolution.Core.Domain.Entities;
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

        public OrderService(
            IOrderRepository orderRepository,
            IGenericRepository<Product> productRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool Success, string Message, OrderDto Order)> CreateOrderAsync(int userId, OrderCreateDto createDto)
        {
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            // 1. Stok ve Fiyat Kontrolü
            foreach (var item in createDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                // Ürün yoksa veya stok yetersizse
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    return (false, $"Ürün bulunamadı veya '{product?.Name ?? "Bilinmeyen Ürün"}' için stok yetersiz. İstek: {item.Quantity}, Mevcut: {product?.StockQuantity ?? 0}", null);
                }

                // OrderItem nesnesini oluştur
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPriceAtPurchase = product.Price // Satın alındığı anki fiyatı kaydet
                };
                orderItems.Add(orderItem);

                totalAmount += orderItem.UnitPriceAtPurchase * item.Quantity;

                // 2. Stoktan Düşme (Tracked Entity'yi güncelle)
                product.StockQuantity -= item.Quantity;
                _productRepository.Update(product);
            }

            // 3. Sipariş (Order) Entity'sini oluştur
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = createDto.ShippingAddress,
                TotalAmount = totalAmount,
                Status = "Pending",
                PaymentCompleted = false,
                OrderItems = orderItems
            };

            // 4. Veritabanına kaydet
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(); // Tüm değişiklikler (Order ve Product stokları) tek transaction'da kaydedilir.

            // 5. Başarılı yanıt DTO'su döndür
            var orderDto = MapOrderToOrderDto(order);

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
            // (Basitlik için şimdilik sadece string ataması yapıyoruz)
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