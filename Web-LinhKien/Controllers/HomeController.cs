using Microsoft.AspNetCore.Mvc;
using Web_LinhKien.Data; // Thêm namespace này
using Microsoft.EntityFrameworkCore; // Thêm namespace này để dùng ToListAsync()

namespace Web_LinhKien.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context; // Khai báo context

        public HomeController(AppDbContext context) // Tiêm AppDbContext vào constructor
        {
            _context = context;
        }

        public async Task<IActionResult> Index() // Thay đổi sang async Task và dùng await
        {
            var products = await _context.Products.ToListAsync(); // Lấy tất cả sản phẩm
            return View(products); // Truyền danh sách sản phẩm sang View
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}