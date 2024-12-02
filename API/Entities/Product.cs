using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string Name { get; set; } = default!;
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = default!;
        public int Quantity { get; set; } = default!;
        public bool Status { get; set; } = true;
        public decimal Price { get; set; }
        public AppUser Vendor { get; set; } = null!;
        public string? Description { get; set; } = default;
        public Category Category { get; set; } = null!;
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}