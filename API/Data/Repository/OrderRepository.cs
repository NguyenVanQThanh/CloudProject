using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Entities.Enum;
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

        public async Task<ICollection<OrderDTO>> GetAllOrders()
        {
            var query = _context.Orders.Include(x=>x.UserVendor)
                        .Include(x=>x.UserClient)
                        .Include(x=>x.OrderDetails)
                        .AsQueryable();
            
            return await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<ICollection<OrderDTO>> GetOrderByClientName(string clientName, OrderStatus? status)
        {
            var query = _context.Orders.Where(o=>o.NameClient == clientName)
                        .Include(x=>x.UserVendor)
                        .Include(x=>x.UserClient)
                        .Include(x=>x.OrderDetails)
                        .AsQueryable();
            if (status.HasValue){
                query = query.Where(o=>o.Status == status.Value);
            }
            return await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _context.Orders.FindAsync(orderId);
        }

        public async Task<ICollection<OrderDTO>> GetOrderByVendorName(string vendorName, OrderStatus? status)
        {
            var query = _context.Orders.Where(o=>o.NameVendor == vendorName)
                        .Include(x=>x.UserVendor)
                        .Include(x=>x.UserClient)
                        .Include(x=>x.OrderDetails)
                        .AsQueryable();
            if(status.HasValue){
                query = query.Where(o=>o.Status == status.Value);
            }
            return await query.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).ToListAsync();
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