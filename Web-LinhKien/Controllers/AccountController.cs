// File: Web-LinhKien/Controllers/AccountController.cs

using Microsoft.AspNetCore.Mvc;
using Web_LinhKien.Models;
using Web_LinhKien.Data; // Có thể không cần thiết trực tiếp trong controller này nếu chỉ dùng UserManager/SignInManager
using Microsoft.AspNetCore.Identity;
using Web_LinhKien.Services; // Đảm bảo namespace này đúng

namespace Web_LinhKien.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
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
                // Sử dụng EmailOrPhoneNumber làm UserName, Email và PhoneNumber
                var user = new IdentityUser { UserName = model.EmailOrPhoneNumber, Email = model.EmailOrPhoneNumber, PhoneNumber = model.EmailOrPhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Gán role mặc định cho người dùng mới
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
                // Tìm người dùng bằng UserName (có thể là Email hoặc Số điện thoại)
                var user = await _userManager.FindByNameAsync(model.EmailOrPhoneNumber);
                
                if (user != null)
                {
                    // Đăng nhập người dùng
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);
                    
                    if (result.Succeeded)
                    {
                        // Kiểm tra vai trò của người dùng
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            // Chuyển hướng đến Dashboard Admin
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                        }
                        else // Nếu không phải Admin (ví dụ: Customer)
                        {
                            // Chuyển hướng đến trang gốc nếu có returnUrl hợp lệ, nếu không thì về trang chủ
                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                // Nếu đăng nhập không thành công hoặc người dùng không tồn tại
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
        
        // Bạn có thể thêm action ResetPassword ở đây nếu cần
        // [HttpGet]
        // public IActionResult ResetPassword(string email, string token)
        // {
        //     if (email == null || token == null)
        //     {
        //         return RedirectToAction("Login");
        //     }
        //     var model = new ResetPasswordViewModel { Email = email, Token = token };
        //     return View(model);
        // }

        // [HttpPost]
        // public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var user = await _userManager.FindByEmailAsync(model.Email);
        //         if (user == null)
        //         {
        //             // Don't reveal that the user does not exist
        //             return RedirectToAction("ResetPasswordConfirmation");
        //         }
        //         var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        //         if (result.Succeeded)
        //         {
        //             return RedirectToAction("ResetPasswordConfirmation");
        //         }
        //         foreach (var error in result.Errors)
        //         {
        //             ModelState.AddModelError(string.Empty, error.Description);
        //         }
        //     }
        //     return View(model);
        // }

        // [HttpGet]
        // public IActionResult ResetPasswordConfirmation()
        // {
        //     return View();
        // }
    }
}