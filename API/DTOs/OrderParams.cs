using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.Enum;
using API.Helpers;

namespace API.DTOs
{
    public class OrderParams : PaginationParams
    {
        public string? ClientName { get; set; } 
        public string? VendorName { get; set; }
        public string? Status {get; set; }
    }
}