using System.ComponentModel.DataAnnotations;

namespace Web_LinhKien.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email hoặc Số điện thoại là bắt buộc.")]
        public string EmailOrPhoneNumber { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}