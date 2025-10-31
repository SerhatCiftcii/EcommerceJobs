

using Application.Dtos.OrdersDto;
using Application.Dtos.OrdersItemDtos;
using ECommerceSolution.Core.Application.DTOs;
using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using ECommerceSolution.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IOrderService _orderService; // Checkout için
        private readonly IUnitOfWork _unitOfWork;

        public CartService(
            ICartRepository cartRepository,
            IGenericRepository<Product> productRepository,
            IOrderService orderService,
            IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _orderService = orderService;
            _unitOfWork = unitOfWork;
        }

        // --- DTO Manuel Dönüşüm Metotları ---

        private CartDto MapToCartDto(Cart cart)
        {
            return new CartDto
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name ?? "Bilinmeyen Ürün",
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product?.Price ?? 0 // Anlık fiyatı al
                }).ToList()
            };
        }

        // --- Service Metotları ---

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartWithDetailsByUserIdAsync(userId);
            return cart != null ? MapToCartDto(cart) : null;
        }

        public async Task<(bool Success, string Message, CartDto Cart)> AddOrUpdateItemAsync(int userId, CartItemManipulationDto itemDto)
        {
            // 1. Ürünü ve stok durumunu kontrol et
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null)
            {
                return (false, "Ürün bulunamadı.", null);
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                return (false, $"Stok yetersiz. Mevcut: {product.StockQuantity}", null);
            }

            // 2. Kullanıcının sepetini getir veya yeni bir tane oluştur
            var cart = await _cartRepository.GetCartWithDetailsByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };

                await _cartRepository.AddAsync(cart);

                // ✅ ID'nin oluşması için hemen kaydediyoruz
                await _unitOfWork.SaveChangesAsync();
            }

            // 3. Sepette bu ürün zaten var mı kontrol et
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == itemDto.ProductId);

            if (cartItem == null)
            {
                // Ürün yeni ekleniyor
                cartItem = new CartItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                cart.CartItems.Add(cartItem);
            }
            else
            {
                // Ürün zaten sepette, miktar güncelleniyor
                cartItem.Quantity += itemDto.Quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;
            }

            // Sepet güncelleme zamanı
            cart.UpdatedAt = DateTime.UtcNow;

            // EF Core sepeti ve item’larını zaten takip ediyor, Update() gerekmez
            await _unitOfWork.SaveChangesAsync();

            // Güncel sepeti yeniden yükle (detaylarla birlikte)
            var updatedCart = await _cartRepository.GetCartWithDetailsByUserIdAsync(userId);

            return (true, "Ürün sepete eklendi veya güncellendi.", MapToCartDto(updatedCart));
        }


        public async Task<bool> RemoveItemAsync(int userId, int cartItemId)
        {
            var cart = await _cartRepository.GetCartWithDetailsByUserIdAsync(userId);

            if (cart == null) return false;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

            if (cartItem == null) return false;

            // Sepet ıtem kaldır ve Sepeti güncelle
            cart.CartItems.Remove(cartItem);
            _cartRepository.Update(cart);

            cart.UpdatedAt = DateTime.UtcNow;

            // EF Core, kaldırdığımız CartItem'ı veritabanından siler.
            var affectedRows = await _unitOfWork.SaveChangesAsync();
            return affectedRows > 0;
        }

        public async Task<(bool Success, string Message, OrderDto Order)> CheckoutAsync(int userId, string shippingAddress)
        {
            var cart = await _cartRepository.GetCartWithDetailsByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return (false, "Sepetiniz boş.", null);
            }

            // 1. Sepet verisini OrderService'in anlayacağı OrderCreateDto formatına dönüştür
            var orderCreateDto = new OrderCreateDto
            {
                ShippingAddress = shippingAddress,
                Items = cart.CartItems.Select(ci => new OrderItemCreateDto
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity
                }).ToList()
            };

            // 2. OrderService'i kullanarak sipariş oluştur
            // OrderService, kendi içinde stok kontrolünü ve Order/OrderItem kaydını yapacaktır.
            var (orderSuccess, orderMessage, orderDto) = await _orderService.CreateOrderAsync(userId, orderCreateDto);

            if (!orderSuccess)
            {
                // Sipariş oluşturma başarısız olursa (Örn: Stok yetersizliği)
                return (false, $"Sipariş oluşturulamadı: {orderMessage}", null);
            }

            // 3. Başarılı Sipariş Sonrası Sepeti Temizle (Kaldır)
            // Sepet ve tüm kalemleri veritabanından silinir (OnDelete(Cascade) nedeniyle)
            _cartRepository.Remove(cart);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Sipariş başarıyla oluşturuldu ve sepetiniz temizlendi.", orderDto);
        }
    }
}