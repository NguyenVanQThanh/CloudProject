using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.Enum;
using API.Interfaces;
using AutoMapper;

namespace API.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<bool> UpdateStatus(int id, string status)
        {
            var orderInDb = await _unitOfWork.OrderRepository.GetOrderById(id) ?? throw new Exception("Order not found");
            switch (status){
                case "InProgress":
                    if (!nameof(orderInDb.Status).Equals("Pending")) throw new Exception("Order status has already not been pending");
                    orderInDb.Status = OrderStatus.InProgress;
                    break;
                case "Completed":
                    if (!nameof(orderInDb.Status).Equals("InProgress")) throw new Exception("Order status has already not been in progress");
                    orderInDb.Status = OrderStatus.Completed;
                    break;
                case "Cancelled":
                    if (!nameof(orderInDb.Status).Equals("Canceled")) throw new Exception("Order status has already been canceled");
                    orderInDb.Status = OrderStatus.Canceled;
                    break;
                default:
                    throw new Exception("Something went wrong");
            }
            if (await _unitOfWork.OrderRepository.UpdateOrder(orderInDb)) return true;
            return false;
        }
    }
}