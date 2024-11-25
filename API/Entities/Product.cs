using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = Datetime.Now;
        public DateTime UpdatedAt { get; set; } = default!;
        public decimal Price { get; set; }
        public AppUser Vendor { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
    }
}