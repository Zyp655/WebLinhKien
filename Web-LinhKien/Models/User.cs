using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_LinhKien.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [MaxLength(100)]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }

        // Ví dụ về Role (Admin, Customer)
        [Required]
        [MaxLength(50)]
        public string Role { get; set; }
        
        // Navigation properties
        public ICollection<Order> Orders { get; set; }
    }
}