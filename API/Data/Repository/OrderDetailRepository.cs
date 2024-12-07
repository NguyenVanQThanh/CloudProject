using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
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

        public Task<PagedList<OrderDetailDTO>> GetOrderDetailsByOrderId(OrderDetailParam orderDetailParam)
        {
            var orderDetail = _context.OrderDetails
                                .Include(od=>od.Order)
                                .Include(od=>od.Product)
                                .AsQueryable();
            if (orderDetailParam.OrderId.HasValue)
            {
                orderDetail = orderDetail.Where(od=>od.OrderId == orderDetailParam.OrderId);
            }
            if (orderDetailParam.ProductId.HasValue){
                orderDetail = orderDetail.Where(od=>od.ProductId == orderDetailParam.ProductId);
            }
            var query = orderDetail.ProjectTo<OrderDetailDTO>(_mapper.ConfigurationProvider);
            return PagedList<OrderDetailDTO>.CreateAsync(query, orderDetailParam.PageNumber, orderDetailParam.PageSize);
        }
    }
}