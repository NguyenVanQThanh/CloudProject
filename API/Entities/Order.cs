using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.Enum;

namespace API.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int IdClient { get; set; }
        public int IdVendor { get; set; }
        public string NameClient { get; set; } = default!;
        public string NameVendor { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? FinishedAt { get; set; }
        public OrderStatus Status { get; set; }
        public OrderPayment Payment { get; set; }
        public AppUser UserClient { get; set; } = null!;
        public AppUser UserVendor { get; set; } = null!;
        public List<OrderDetail>? OrderDetails { get; set; } 
        
    }
}