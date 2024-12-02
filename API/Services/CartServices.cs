using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

namespace API.Services
{
    public class CartServices : ICartServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public CartServices(IMapper mapper, DataContext context, IUnitOfWork unitOfWork){
            _mapper = mapper;
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> AddToCart(AddToCartDTO addToCartDTO)
        {
            var product = await _unitOfWork.ProductRepository.GetProductById(addToCartDTO.ProductId);
            if (product == null){
                throw new Exception("Product does not exist");
            }
            if (product.Quantity < addToCartDTO.Quantity || !product.Status){
                throw new Exception("Product is not available");
            }
            var client = await _unitOfWork.UserRepository.GetUserByUserName(addToCartDTO.ClientName);
            if (client == null)
            {
                throw new Exception("Client does not exist");
            }

            var vendor = await _unitOfWork.UserRepository.GetUserByUserName(addToCartDTO.VendorName);
            if (vendor == null)
            {
                throw new Exception("Vendor does not exist");
            }
            var cart = await _unitOfWork.CartRepository.GetCartDetail(addToCartDTO.ClientName,addToCartDTO.VendorName);
            if (cart == null)
            {
                // Tạo giỏ hàng mới nếu chưa có
                cart = new Cart
                {
                    ClientId = client.Id,
                    VendorId = vendor.Id,
                    DateCreated = DateTime.Now,
                    TotalPrice = 0, // Có thể tính giá trị ban đầu của giỏ hàng nếu cần
                    Client = client,
                    Vendor = vendor
                };

                // Lưu giỏ hàng vào cơ sở dữ liệu
                _unitOfWork.CartRepository.AddCart(cart);
                _unitOfWork.HasChanges();
            }
            var cartItem = await _unitOfWork.CartItemRepository.GetCartItemsAsync(cart.Id, addToCartDTO.ProductId);
            if (cartItem == null)
            {
                cartItem = new CartItem{
                    Cart = cart,
                    Product = product,
                    Quantity = addToCartDTO.Quantity,
                    Price = product.Price * addToCartDTO.Quantity,
                };
            } else{
                cartItem.Quantity += addToCartDTO.Quantity;
                cartItem.Price = product.Price * cartItem.Quantity;
            }
            cart.TotalPrice += cartItem.Price;
            await _unitOfWork.CartRepository.UpdateCart(cart);
            _unitOfWork.CartItemRepository.AddCartItem(cartItem);
            await _unitOfWork.Complete();
            return true;
        }
        
    }
}