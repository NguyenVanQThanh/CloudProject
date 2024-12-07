using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.Enum;

namespace API.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int IdClient { get; set; }
        public int IdVendor { get; set; }
        public string NameClient { get; set; } = default!;
        public string NameVendor { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string Status { get; set;} = default!;
        public string Payment { get; set; } = default!;
        public List<OrderDetailDTO> OrderDetails { get; set; } = new();
    }
}