using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = default!;
        public int CategoryId { get; set; }
        public IFormFile Image { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}