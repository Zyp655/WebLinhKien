using Microsoft.AspNetCore.Mvc;
using Web_LinhKien.Models;
using Web_LinhKien.Data;
using Microsoft.AspNetCore.Identity; 
using Web_LinhKien.Services; 
using System.Diagnostics;
using Microsoft.AspNetCore.Http; 
using System.Linq; 

namespace Web_LinhKien.Controllers
{
    public class AccountController : Controller
    {
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
                    return RedirectToAction("Login", "Account");
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
                var user = await _userManager.FindByEmailAsync(model.EmailOrPhoneNumber) ?? await _userManager.FindByNameAsync(model.EmailOrPhoneNumber);
                
                if (user != null)
                {
                    var code = new Random().Next(100000, 999999).ToString(); 
                    
                    HttpContext.Session.SetString("PasswordResetCode_" + user.Id.ToString(), code);
                    HttpContext.Session.SetString("PasswordResetUserId", user.Id.ToString()); 
                    HttpContext.Session.SetString("PasswordResetUserEmailOrPhone_" + user.Id.ToString(), user.Email ?? user.PhoneNumber); 

                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        await _emailService.SendEmailAsync(user.Email, "Mã xác thực đặt lại mật khẩu", $"Mã xác thực của bạn là: <b>{code}</b>. Mã này sẽ hết hạn sau vài phút.");
                        return Json(new { success = true, message = "Mã xác thực đã được gửi đến email của bạn.", userId = user.Id.ToString() });
                    }
                    else if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        return Json(new { success = false, message = "Tính năng gửi mã xác thực qua số điện thoại chưa được triển khai." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không thể gửi mã xác thực. Tài khoản không có email hoặc số điện thoại hợp lệ." });
                    }
                }
                return Json(new { success = true, message = "Nếu tài khoản của bạn tồn tại, một mã xác thực đã được gửi." }); 
            }
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                                   .ToDictionary(
                                       kvp => kvp.Key,
                                       kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                                   );
            return Json(new { success = false, message = "Dữ liệu nhập không hợp lệ.", errors = errors });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Mã xác thực không hợp lệ." });
            }

            var userId = HttpContext.Session.GetString("PasswordResetUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Phiên xác thực đã hết hạn hoặc không hợp lệ. Vui lòng thử lại quy trình quên mật khẩu." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại hoặc phiên đã hết hạn." });
            }

            var storedCode = HttpContext.Session.GetString("PasswordResetCode_" + user.Id.ToString());

            if (string.IsNullOrEmpty(storedCode) || storedCode != model.VerificationCode)
            {
                return Json(new { success = false, message = "Mã xác thực không đúng hoặc đã hết hạn." });
            }
            
            return Json(new { success = true, redirectToUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id.ToString() }) });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "" }); 
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId) 
        {
            if (userId == null)
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = "Phiên đặt lại mật khẩu không hợp lệ hoặc đã hết hạn. Vui lòng thử lại." }); 
            }

            var sessionUserId = HttpContext.Session.GetString("PasswordResetUserId");
            if (sessionUserId != userId)
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = "Truy cập không hợp lệ. Vui lòng bắt đầu lại quy trình đặt lại mật khẩu." });
            }

            var userEmailOrPhone = HttpContext.Session.GetString("PasswordResetUserEmailOrPhone_" + userId); 
            if (string.IsNullOrEmpty(userEmailOrPhone))
            {
                return View("~/Views/Shared/Error.cshtml", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = "Phiên đặt lại mật khẩu đã hết hạn. Vui lòng thử lại." }); 
            }

            var model = new ResetPasswordViewModel { Email = userEmailOrPhone, UserId = userId }; 
            return View(model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                                       .ToDictionary(
                                           kvp => kvp.Key,
                                           kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                                       );
                string generalMessage = "Vui lòng kiểm tra lại thông tin bạn đã nhập.";
                return Json(new { success = false, message = generalMessage, errors = errors });
            }

            var userId = model.UserId; 
            if (string.IsNullOrEmpty(userId))
            {
                userId = HttpContext.Session.GetString("PasswordResetUserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Phiên đặt lại mật khẩu đã hết hạn. Vui lòng thử lại quy trình quên mật khẩu." });
                }
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại hoặc phiên đã hết hạn. Vui lòng thử lại." });
            }

            var sessionEmailOrPhone = HttpContext.Session.GetString("PasswordResetUserEmailOrPhone_" + user.Id.ToString());
            if (string.IsNullOrEmpty(sessionEmailOrPhone) || 
                (!string.Equals(sessionEmailOrPhone.Trim(), model.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                 return Json(new { success = false, message = "Email/Số điện thoại không khớp với tài khoản trong phiên xác thực." });
            }


            var storedCode = HttpContext.Session.GetString("PasswordResetCode_" + user.Id.ToString());
            if (string.IsNullOrEmpty(storedCode) || storedCode != model.VerificationCode) 
            {
                 return Json(new { success = false, message = "Mã xác thực không hợp lệ hoặc đã hết hạn." });
            }
            
            HttpContext.Session.Remove("PasswordResetCode_" + user.Id.ToString());

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

            if (result.Succeeded)
            {
                HttpContext.Session.Remove("PasswordResetUserEmailOrPhone_" + user.Id.ToString());
                HttpContext.Session.Remove("PasswordResetUserId");

                return Json(new { success = true, redirectToUrl = Url.Action("ResetPasswordSuccess", "Account") }); 
            }

            var identityErrors = result.Errors.Select(e => e.Description).ToList();
            return Json(new { success = false, message = "Đặt lại mật khẩu thất bại.", errors = new { Password = identityErrors } });
        }
        [HttpGet]
        public IActionResult ResetPasswordSuccess()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

       
    }
}