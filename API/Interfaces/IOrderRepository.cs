using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Entities.Enum;
using API.Helpers;

namespace API.Interfaces
{
    public interface IOrderRepository
    {
        Task<PagedList<OrderDTO>> GetAllOrders(OrderParams orderParams);
        Task<Order> GetOrderById(int orderId);
        void AddOrder(Order order);
        Task<bool> UpdateOrder(Order order);
        Task<Order> GetOrderDetail(string clientName, string vendorName);
    }
}