using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Web_LinhKien.Models;
using System.Collections.Generic;

namespace Web_LinhKien.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

       
            var adminRoleId = 1; 
            var adminUserId = 1; 

            builder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int>
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "STATIC_ADMIN_ROLE_CONCURRENCY_STAMP"
            });

            var passwordHasher = new PasswordHasher<User>(); 
            var adminUser = new User
            {
                Id = adminUserId,
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");

            builder.Entity<User>().HasData(adminUser);

            builder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });
            
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Hàng thanh lý" },
                new Category { Id = 2, Name = "Linh kiện điện tử" },
                new Category { Id = 3, Name = "VDK-IC chức năng" },
                new Category { Id = 4, Name = "Module -Cảm Biến" },
                new Category { Id = 5, Name = "Phụ Kiện Điện Tử" },
                new Category { Id = 6, Name = "Kết Nối" },
                new Category { Id = 7, Name = "LED-LCD-Đèn báo" },
                new Category { Id = 8, Name = "Pin-Nguồn" }, 
                new Category { Id = 9, Name = "Thiết bị - Phụ Trợ" },
                new Category { Id = 10, Name = "Điện tử ứng dụng" }
            );

            // <--- THÊM DỮ LIỆU SẢN PHẨM MẪU VÀO ĐÂY --->
            builder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Arduino UNO R3", Description = "Board phát triển Arduino phổ biến", Price = 150000m, ImageUrl = "/images/arduino_uno.jpg", CategoryId = 2 },
                new Product { Id = 2, Name = "Cảm Biến Nhiệt Độ DS18B20", Description = "Cảm biến nhiệt độ kỹ thuật số chống nước", Price = 35000m, ImageUrl = "/images/ds18b20.jpg", CategoryId = 4 },
                new Product { Id = 3, Name = "Mô-đun ESP8266 ESP-01S", Description = "Module Wi-Fi giá rẻ, dễ sử dụng", Price = 45000m, ImageUrl = "/images/esp8266.jpg", CategoryId = 4 },
                new Product { Id = 4, Name = "Điện trở 10K Ohm (1/4W)", Description = "Gói 100 chiếc điện trở 10K Ohm", Price = 10000m, ImageUrl = "/images/resistor_10k.jpg", CategoryId = 5 },
                new Product { Id = 5, Name = "Led Đơn 5mm Xanh Lá", Description = "Gói 50 chiếc Led đơn 5mm màu xanh lá", Price = 8000m, ImageUrl = "/images/led_green.jpg", CategoryId = 7 }
            );
            // <--- KẾT THÚC DỮ LIỆU SẢN PHẨM MẪU --->
        }
    }
}