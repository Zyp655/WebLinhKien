using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Web_LinhKien.Data
{
    public static class SeedData
    {
        // Hàm khởi tạo để tạo các Role
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // Danh sách các quyền bạn muốn có trong hệ thống
            string[] roleNames = { "ADMIN", "CUSTOMER" };

            foreach (var roleName in roleNames)
            {
                // Kiểm tra xem quyền đã tồn tại chưa
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }
        }
    }
}