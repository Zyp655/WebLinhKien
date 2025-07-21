using System.ComponentModel.DataAnnotations;

namespace Web_LinhKien.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; }
    }
}