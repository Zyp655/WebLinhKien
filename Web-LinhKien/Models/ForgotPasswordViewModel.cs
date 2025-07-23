// File: Web_LinhKien/Models/ForgotPasswordViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Web_LinhKien.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Email hoặc Số điện thoại.")]
        [Display(Name = "Email hoặc Số điện thoại")]
        // Bạn có thể thêm validation tùy chỉnh nếu muốn kiểm tra định dạng email hoặc số điện thoại
        // Hoặc sử dụng EmailAddress cho email
        public string EmailOrPhoneNumber { get; set; }
    }
}