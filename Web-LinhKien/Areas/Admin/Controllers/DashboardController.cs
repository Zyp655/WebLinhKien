using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web_LinhKien.Areas.Admin.Models;
using Web_LinhKien.Data;
using Microsoft.EntityFrameworkCore;

namespace Web_LinhKien.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount),
                PendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending")
            };
            return View(viewModel);
        }
    }
}