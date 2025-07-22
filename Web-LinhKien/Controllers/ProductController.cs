using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_LinhKien.Data;
using Web_LinhKien.Models;

namespace Web_LinhKien.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
          
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Detail(int id)
        {
          
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}