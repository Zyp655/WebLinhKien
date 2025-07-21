using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web_LinhKien.Areas.Admin.Models
{
    public class AdminProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Trường này để hiển thị tên tệp ảnh khi upload
        public IFormFile ImageFile { get; set; } 

        [Required]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        // Danh sách các danh mục để tạo dropdown list
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}