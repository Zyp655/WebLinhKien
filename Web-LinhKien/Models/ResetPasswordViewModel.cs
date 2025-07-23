// Web_LinhKien/Models/ResetPasswordViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Web_LinhKien.Models
{
    public class ResetPasswordViewModel
    {
        // Thêm thuộc tính UserId vào đây
        public string UserId { get; set; } 

        [Required(ErrorMessage = "Email hoặc Số điện thoại là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Mã xác thực là bắt buộc.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã xác thực phải có 6 chữ số.")]
        [DataType(DataType.Text)] 
        public string VerificationCode { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}