using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task<PagedList<OrderDetailDTO>> GetOrderDetailsByOrderId(OrderDetailParam orderDetailParam);
        void AddOrderDetail(OrderDetail orderDetail);
    }
}