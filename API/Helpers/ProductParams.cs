using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class ProductParams : PaginationParams
    {
        public string? ProductName {get; set; }
        public string? VendorName {get; set; }
        public string? CategoryName { get; set; } 
        public bool? Status { get; set; }
        public decimal? MinPrice {get; set;}
        public decimal? MaxPrice {get; set; }
    }
}