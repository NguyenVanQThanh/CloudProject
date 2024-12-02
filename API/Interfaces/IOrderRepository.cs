using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Entities.Enum;

namespace API.Interfaces
{
    public interface IOrderRepository
    {
        Task<ICollection<OrderDTO>> GetAllOrders();
        Task<ICollection<OrderDTO>> GetOrderByVendorName(string vendorName, OrderStatus? status);
        Task<ICollection<OrderDTO>> GetOrderByClientName(string clientName, OrderStatus? status);
        Task<Order> GetOrderById(int orderId);
        void AddOrder(Order order);
        Task<bool> UpdateOrder(Order order);
        Task<Order> GetOrderDetail(string clientName, string vendorName);
    }
}