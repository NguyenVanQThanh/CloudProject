using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Cart {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int VendorId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        // public decimal TotalPrice { get; set; }
        public AppUser Client { get; set;} = default!;
        public AppUser Vendor { get; set;} = default!;
        public List<CartItem> CartItems { get; set; } = new();
    }
}