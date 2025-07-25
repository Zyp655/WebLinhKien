using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web_LinhKien.Areas.Admin.Models;
using Web_LinhKien.Data;
using Microsoft.EntityFrameworkCore;
using Web_LinhKien.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Hosting; // Đảm bảo có namespace này

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                string fileName = null;
                if (model.ImageFile != null)
                {
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    // Đường dẫn tuyệt đối để lưu ảnh trong wwwroot/images/products
                    var filePath = Path.Combine(_env.WebRootPath, "images", "products", fileName);

                    // Đảm bảo thư mục tồn tại
                    var directoryPath = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }
                }

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    // Lưu đường dẫn tương đối để dễ hiển thị trên web
                    ImageUrl = fileName != null ? "/images/products/" + fileName : null
                };

                try
                {
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Sản phẩm đã được thêm mới thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                   
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu sản phẩm: " + ex.Message);
                  
                }
            }

           
            model.Categories = await _context.Categories
                                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                                        .ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new AdminProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
              
                Categories = await _context.Categories
                                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                                        .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminProductViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var productToUpdate = await _context.Products.FindAsync(id);
                if (productToUpdate == null)
                {
                    return NotFound();
                }

                productToUpdate.Name = model.Name;
                productToUpdate.Description = model.Description;
                productToUpdate.Price = model.Price;
                productToUpdate.CategoryId = model.CategoryId;

               
                if (model.ImageFile != null)
                {
                    if (!string.IsNullOrEmpty(productToUpdate.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_env.WebRootPath, productToUpdate.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Lưu ảnh mới
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    var filePath = Path.Combine(_env.WebRootPath, "images", "products", fileName);

                    var directoryPath = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }
                    productToUpdate.ImageUrl = "/images/products/" + fileName;
                }

                try
                {
                    _context.Update(productToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(productToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; 
                    }
                }
                catch (Exception ex)
                {
                    
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật sản phẩm: " + ex.Message);
                }
            }

            
            model.Categories = await _context.Categories
                                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                                        .ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagePath = Path.Combine(_env.WebRootPath, product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa sản phẩm: " + ex.Message;
               
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}