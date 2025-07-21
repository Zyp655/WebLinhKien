using Microsoft.AspNetCore.Mvc;

namespace Web_LinhKien.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Logic để lấy dữ liệu sản phẩm nổi bật, tin tức mới...
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}