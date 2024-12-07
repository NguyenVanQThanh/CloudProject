using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CartDTOs
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = default!;
        public string VendorName { get; set; } = default!;
        public decimal TotalPrice { get; set; }
        public DateTime DateCreated { get; set; }
        public List<CartItemDTO> CartItems { get; set; } = new();
    }
}