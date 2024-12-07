using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;

namespace API.DTOs
{
    public class OrderDetailParam : PaginationParams
    {
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
    }
}