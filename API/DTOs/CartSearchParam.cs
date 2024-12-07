using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;

namespace API.DTOs
{
    public class CartSearchParam : PaginationParams
    {
        public string ClientName { get; set; } = "";
        public string? VendorName { get; set; }
    }
}