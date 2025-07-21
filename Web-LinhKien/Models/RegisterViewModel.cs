using System.ComponentModel.DataAnnotations;

namespace Web_LinhKien.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email hoặc Số điện thoại là bắt buộc.")]
        public string EmailOrPhoneNumber { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}