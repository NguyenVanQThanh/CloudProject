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
using Microsoft.IdentityModel.Tokens;

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
            if (addToCartDTO.ClientName.Equals(addToCartDTO.VendorName)){
                throw new Exception("Client and Vendor can't be the same");
            }
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
                    Client = client,
                    Vendor = vendor
                };

                // Lưu giỏ hàng vào cơ sở dữ liệu
                _unitOfWork.CartRepository.AddCart(cart);
            }
            var cartItem = await _unitOfWork.CartItemRepository.GetCartItemsAsync(cart.Id, addToCartDTO.ProductId);
            if (cartItem == null)
            {
                cartItem = new CartItem{
                    Cart = cart,
                    Product = product,
                    Quantity = addToCartDTO.Quantity,
                };
            } else{
                cartItem.Quantity += addToCartDTO.Quantity;
                cartItem.Product.Price = product.Price * cartItem.Quantity;
            }
            _unitOfWork.CartRepository.UpdateCart(cart);
            _unitOfWork.CartItemRepository.AddCartItem(cartItem);
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> ClearCart(string clientName)
        {
            var cartInDB = await _unitOfWork.CartRepository.GetCartByClient(clientName);
            if (cartInDB != null){
                foreach(var cart in cartInDB){
                    _unitOfWork.CartRepository.DeleteCart(cart);
                }
            }
            return true;
        }

        public async Task<bool> RemoveFromCart(int productId, int cartId)
        {
            var cartItemInDB = await _unitOfWork.CartItemRepository.GetCartItemsAsync(cartId,productId);
            if (cartItemInDB == null) {
                throw new Exception("Item not found in cart");
            }
            var cartInDB = await _unitOfWork.CartRepository.GetCartById(cartId);
            if (cartInDB == null) {
                throw new Exception("Cart not found");
            }
            _unitOfWork.CartItemRepository.DeleteCartItem(productId,cartId);
            if (cartInDB.CartItems.IsNullOrEmpty()){
                _unitOfWork.CartRepository.DeleteCart(cartInDB);
            }
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> UpdateQuantity(CartItemDTO cartItemDTO)
        {
            var cartInDB = await _unitOfWork.CartRepository.GetCartById(cartItemDTO.CartId);
            if (cartInDB == null) {
                throw new Exception("Cart not found");
            }
            var cartItemInDB = await _unitOfWork.CartItemRepository.GetCartItemsAsync(cartItemDTO.ProductId, cartItemDTO.CartId);
            if (cartItemInDB == null) {
                throw new Exception("Item not found in cart");
            }
            if (cartItemDTO.Quantity <= 0 || cartItemDTO.Quantity > cartItemInDB.Product.Quantity){
                throw new Exception("Invalid quantity");
            }
            cartItemInDB.Quantity = cartItemDTO.Quantity;
            _unitOfWork.CartItemRepository.UpdateCartItem(cartItemInDB);
            _unitOfWork.CartRepository.UpdateCart(cartInDB);
            await _unitOfWork.Complete();
            return true;
        }
    }
}