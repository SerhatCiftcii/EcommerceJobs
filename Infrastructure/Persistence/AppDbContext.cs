

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
        }
    }
}