using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ProductDTOs
    {
        public int Id { get; set;}
        public string Name { get; set;} = default!;
        public string CategoryName { get; set;} = default!;
        public string Vendor { get; set;} = default!;
        public string Description { get; set;} = default!;
        public decimal Price { get; set;}
        public string? ImageUrl { get; set;}
        public int Quantity { get; set;}
        public bool Status { get; set;}

    }
}