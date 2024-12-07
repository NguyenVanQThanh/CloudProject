using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Entities.Enum;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Data.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public OrderRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public async Task<PagedList<OrderDTO>> GetAllOrders(OrderParams orderParams)
        {
            var product = _context.Orders
                        .Include(x=>x.UserVendor)
                        .Include(x=>x.UserClient)
                        .Include(x=>x.OrderDetails)
                        .AsQueryable();
            if (!string.IsNullOrEmpty(orderParams.ClientName)){
                product = product.Where(o=>o.UserClient.UserName == orderParams.ClientName);
            }
            if (!string.IsNullOrEmpty(orderParams.VendorName)){
                product = product.Where(o=>o.UserVendor.UserName == orderParams.VendorName);
            }
            if (!string.IsNullOrEmpty(orderParams.Status)){
                switch (orderParams.Status){
                    case "Pending":
                        product = product.Where(o=>o.Status == OrderStatus.Pending);
                        break;
                    case "Canceled":
                        product = product.Where(o=>o.Status == OrderStatus.Canceled);
                        break;
                    case "Completed":
                        product = product.Where(o=>o.Status == OrderStatus.Completed);
                        break;
                    default:
                        break;
                }
            }
            var query = product.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider);
            return await PagedList<OrderDTO>.CreateAsync(query, orderParams.PageNumber, orderParams.PageSize);
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _context.Orders.FindAsync(orderId);
        }
        public async Task<Order> GetOrderDetail(string clientName, string vendorName)
        {
            return await _context.Orders.Where(o=>o.NameClient == clientName && o.NameVendor == vendorName)
                        .Include(x=>x.UserVendor)
                        .Include(x=>x.UserClient)
                        .Include(x=>x.OrderDetails)
                        .FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateOrder(Order order)
        {
            var orderInDb = await _context.Orders.FindAsync(order.Id);
            if (orderInDb == null) return false;
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}