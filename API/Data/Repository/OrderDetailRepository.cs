using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public OrderDetailRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }

        public async Task<ICollection<OrderDTO>> GetOrdersByProductId(List<int> productIds)
        {
            var query = _context.OrderDetails
                        .Where(od=>productIds.Contains(od.ProductId))
                        .Include(od=>od.Order)
                        .ThenInclude(o=>o.UserClient)
                        .Include(od=>od.Order)
                        .ThenInclude(o=>o.UserVendor)
                        .Include(od=>od.Product)
                        .AsQueryable();
            var orders = query.Select(od=>od.Order).Distinct();
            return await orders.ProjectTo<OrderDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}