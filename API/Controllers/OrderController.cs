using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class OrderController(IUnitOfWork _unitOfWork, IMapper _mapper) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<PagedList<OrderDTO>>> GetOrders([FromQuery] OrderParams orderParams)
        {
            var user = User.GetUserName();
            orderParams.ClientName = user;
            var orders = await _unitOfWork.OrderRepository.GetAllOrders(orderParams);
            Response.AddPaginationHeader(new PaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages));
            return Ok(orders);
        }
        [HttpGet("details")]
        public async Task<ActionResult<PagedList<OrderDetailDTO>>> GetOrderById([FromQuery] OrderDetailParam orderDetailParam){
            var orderDetail = await _unitOfWork.OrderDetailRepository.GetOrderDetailsByOrderId(orderDetailParam);
            Response.AddPaginationHeader(new PaginationHeader(orderDetail.CurrentPage,orderDetail.PageSize, orderDetail.TotalCount, orderDetail.TotalPages));
            return Ok(orderDetail);
        }
        
    }
}