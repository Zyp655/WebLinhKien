using System.ComponentModel.DataAnnotations;

namespace Web_LinhKien.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email hoặc Số điện thoại là bắt buộc.")]
        public string EmailOrPhoneNumber { get; set; }
    }
}