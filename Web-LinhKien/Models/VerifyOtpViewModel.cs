// Web_LinhKien/Models/VerifyOtpViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Web_LinhKien.Models
{
    public class VerifyOtpViewModel
    {
        [Required(ErrorMessage = "Mã xác thực là bắt buộc.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã xác thực phải có 6 chữ số.")]
        [Display(Name = "Mã xác thực (OTP)")]
        public string VerificationCode { get; set; }
        
        // Có thể thêm UserId vào đây nếu bạn muốn gửi từ client, nhưng dùng Session an toàn hơn
        // public string UserId { get; set; } 
    }
}