

using ECommerceSolution.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ECommerceSolution.Infrastructure.Persistence
{
   
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<DailyReport> DailyReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Role Enum Mapping (string olarak saklamak için)
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>() // Enum'ı veritabanında string olarak sakla
                .HasMaxLength(20);

            // Product ve Category Arasındaki İlişki (One-to-Many)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                // Kategori silinirse, o kategoriye ait ürünlerin silinmesini engelle (Restrict)
                .OnDelete(DeleteBehavior.Restrict);

            // Order ve User Arasındaki İlişki (One-to-Many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders) // User entity'sine Orders property'sini eklemiştik
                .HasForeignKey(o => o.UserId)
                // Kullanıcı silinirse siparişleri de sil (Cascade)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem ve Order Arasındaki İlişki (One-to-Many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem ve Product Arasındaki İlişki (One-to-Many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany() // Product'ın OrderItem'lara doğrudan navigasyon property'si yok
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Ürün silinirse, sipariş kalemlerinin korunmasını sağla (Restrict)


            // Cart ve User İlişkisi (1:1/1:Ç - 1 Kullanıcının 1 Sepeti olacak )
            // NOT:  bir kullanıcının sadece tek bir aktif sepeti olacağı için .
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany() // User tarafında Navigation Property zorunlu değil
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinirse sepet de silinsin

            // CartItem ve Cart İlişkisi (Çok:1)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade); // Sepet silinirse, kalemler de silinir.

            // CartItem ve Product İlişkisi (Çok:1)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany() // Product tarafında Navigation Property zorunlu değil
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Ürün silinirse, sepet ıtem silinmesin (veritabanı hatası vermesin, bu iş kuralını silme servisi yönetecek).
                  // DailyReport decimal precision ayarı             
            modelBuilder.Entity<DailyReport>()
                .Property(d => d.TotalSalesAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DailyReport>()
                .Property(d => d.AverageOrderValue)
                .HasPrecision(18, 2);
        }
    }
}