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
                new Category { Id = 8, Name = "Pin-Nguồn" }, // Sửa lỗi chính tả nếu cần
                new Category { Id = 9, Name = "Thiết bị - Phụ Trợ" },
                new Category { Id = 10, Name = "Điện tử ứng dụng" }
            );
        }
    }
}