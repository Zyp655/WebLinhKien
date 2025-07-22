// File: Web_LinhKien/Models/User.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Cần thiết

namespace Web_LinhKien.Models
{
 
    public class User : IdentityUser<int>
    {
        public ICollection<Order> Orders { get; set; }
    }
}