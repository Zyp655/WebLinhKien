// File: Models/CartItem.cs
using System.ComponentModel.DataAnnotations;

namespace Web_LinhKien.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
        
        public string ImageUrl { get; set; }
        
        public decimal Total => Price * Quantity;
    }
}