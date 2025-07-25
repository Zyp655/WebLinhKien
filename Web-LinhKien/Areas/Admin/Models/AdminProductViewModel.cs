using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http; 

namespace Web_LinhKien.Areas.Admin.Models
{
    public class AdminProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [MaxLength(255)]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")] 
        public decimal Price { get; set; }
        
        [Display(Name = "Hình ảnh")] 
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

       
        public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}