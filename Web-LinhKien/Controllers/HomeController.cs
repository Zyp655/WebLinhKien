using Microsoft.AspNetCore.Mvc;
using Web_LinhKien.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Thêm namespace này để dùng GroupBy hoặc các thao tác LINQ khác

namespace Web_LinhKien.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
          
            var categoriesWithProducts = await _context.Categories
                .Include(c => c.Products.OrderBy(p => p.Name)) // Bao gồm sản phẩm và sắp xếp chúng
                .OrderBy(c => c.Id) // Sắp xếp Category nếu cần
                .ToListAsync();

            return View(categoriesWithProducts); 
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}