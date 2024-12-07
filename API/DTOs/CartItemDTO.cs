using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CartItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public int CartId { get; set; } = default!;
        public int Quantity { get; set; }
        public int QuantityInStock { get; set; }
        public decimal Price { get; set; } 
    }
}