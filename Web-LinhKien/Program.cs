// File: Program.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_LinhKien.Data;
using Web_LinhKien.Services; // Đảm bảo namespace này đúng

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Connection String cho SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. Đăng ký AppDbContext với Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Cấu hình ASP.NET Core Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // Thêm hỗ trợ Roles
    .AddEntityFrameworkStores<AppDbContext>();

// 4. Đăng ký Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Cần thiết cho Identity UI

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Cho phép phục vụ các tệp tĩnh từ wwwroot

app.UseRouting();

// 5. Thêm Authentication và Authorization Middleware
app.UseAuthentication(); // Phải đứng trước UseAuthorization
app.UseAuthorization();

// 6. Cấu hình Routing cho Admin Area
app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

// Route mặc định cho các controller công khai
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Cấu hình cho Identity UI (Razor Pages)
app.MapRazorPages();

app.Run();