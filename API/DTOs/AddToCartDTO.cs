using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class AddToCartDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        // public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public string VendorName { get; set; } = default!;
        List<CartItemDTO> CartItems { get; set; } = new();
    }
}