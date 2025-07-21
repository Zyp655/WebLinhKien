using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Web_LinhKien.Models;

namespace Web_LinhKien.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Định nghĩa các DbSet cho các Model của bạn
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

            // Dữ liệu seeding cho người dùng Admin
            var adminRoleId = "a08e1d7a-1563-44f2-9591-873b88939c1d";
            var adminUserId = "b1c2d3e4-f5a6-7b8c-9d0e-1f2a3b4c5d6e";

            // Bước 1: Thêm vai trò (Role) "Admin"
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "STATIC_ADMIN_ROLE_CONCURRENCY_STAMP"
            });

            // Bước 2: Tạo người dùng Admin và băm mật khẩu
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var adminUser = new IdentityUser
            {
                Id = adminUserId,
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = "A1B2C3D4-E5F6-7890-1234-567890ABCDEF",
                ConcurrencyStamp = "STATIC_ADMIN_USER_CONCURRENCY_STAMP"
            };
            // Mật khẩu mặc định: Admin@123. Hãy thay đổi mật khẩu này sau khi chạy lần đầu.
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");

            // Thêm người dùng Admin vào cơ sở dữ liệu
            builder.Entity<IdentityUser>().HasData(adminUser);

            // Bước 3: Liên kết người dùng Admin với vai trò "Admin"
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });
        }
    }
}