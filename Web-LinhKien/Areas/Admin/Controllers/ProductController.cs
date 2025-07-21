using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web_LinhKien.Areas.Admin.Models;
using Web_LinhKien.Data;
using Microsoft.EntityFrameworkCore;
using Web_LinhKien.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web_LinhKien.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories
                                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                                        .ToListAsync();
            var viewModel = new AdminProductViewModel { Categories = categories };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic xử lý upload hình ảnh
                var fileName = Path.GetFileName(model.ImageFile.FileName);
                var filePath = Path.Combine(_env.WebRootPath, "images/products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    ImageUrl = "/images/products/" + fileName
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}