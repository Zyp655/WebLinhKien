// File: Web-LinhKien/Controllers/AccountController.cs

using Microsoft.AspNetCore.Mvc;
using Web_LinhKien.Models;
using Web_LinhKien.Data;
using Microsoft.AspNetCore.Identity; // Đảm bảo namespace này đúng
using Web_LinhKien.Services; // Đảm bảo namespace này đúng

namespace Web_LinhKien.Controllers
{
    public class AccountController : Controller
    {
        // Thay đổi IdentityUser thành User
        private readonly UserManager<User> _userManager; 
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                var user = new User { UserName = model.EmailOrPhoneNumber, Email = model.EmailOrPhoneNumber, PhoneNumber = model.EmailOrPhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    
                    await _userManager.AddToRoleAsync(user, "Customer"); 
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
               
                var user = await _userManager.FindByNameAsync(model.EmailOrPhoneNumber);
                
                if (user != null)
                {
                   
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);
                    
                    if (result.Succeeded)
                    {
                      
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                          
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                
                ModelState.AddModelError(string.Empty, "Đăng nhập không hợp lệ.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tìm người dùng bằng Email hoặc UserName (PhoneNumber)
                var user = await _userManager.FindByEmailAsync(model.EmailOrPhoneNumber) ?? await _userManager.FindByNameAsync(model.EmailOrPhoneNumber);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token }, Request.Scheme);

                    // Đảm bảo user.Email không null trước khi gửi email
                    if (user.Email != null)
                    {
                        await _emailService.SendEmailAsync(user.Email, "Đặt lại mật khẩu", $"Vui lòng click vào liên kết sau để đặt lại mật khẩu của bạn: {resetUrl}");
                    }
                    else
                    {
                        // Xử lý trường hợp email của người dùng là null (nếu họ đăng ký bằng số điện thoại)
                        // Có thể log lỗi hoặc hiển thị thông báo phù hợp
                    }
                }
            }
            // Luôn trả về thông báo thành công để tránh bị tấn công brute-force
            // hoặc thông báo rằng email/số điện thoại đã được xử lý (không tiết lộ liệu tài khoản có tồn tại hay không)
            return View("ForgotPasswordConfirmation");
        }
        
        // Bạn có thể thêm action Logout vào đây
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            // Điều hướng đến trang chủ hoặc trang đăng nhập
            return RedirectToAction("Index", "Home", new { area = "" }); 
        }

        // ... Các hành động ResetPassword khác nếu có
    }
}