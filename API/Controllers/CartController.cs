using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CartController(IUnitOfWork _unitOfWork, ICartServices _cartServices, IMapper _mapper) : BaseApiController
    {
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDTO>>> GetCartItems([FromQuery] CartSearchParam cartSearchParam)
        {
            var user = User.GetUserName();
            cartSearchParam.ClientName = user;
            var cartLists = await _unitOfWork.CartRepository.GetCartByParam(cartSearchParam);
            Response.AddPaginationHeader(new PaginationHeader(cartLists.CurrentPage, cartLists.PageSize, cartLists.TotalPages));
            return Ok(cartLists);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<string>> AddToCart([FromBody] AddToCartDTO addToCartDTO)
        {
            try
            {
                addToCartDTO.ClientName = User.GetUserName();
                var result = await _cartServices.AddToCart(addToCartDTO);
                return Ok("Product added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpDelete("item/{productId}/{cartId}")]
        public async Task<ActionResult> DeleteCartItem(int productId, int cartId)
        {
            var user = User.GetUserId();
            var cartInDB = await _unitOfWork.CartRepository.GetCartById(cartId);
            if(user != cartInDB.ClientId){
                return Unauthorized("You don't have permission to delete cart items");
            }
            try {
                await _cartServices.RemoveFromCart(productId, cartId);
                return Ok("Product removed successfully");
            }catch (Exception ex){
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCart(int id)
        {
            var user = User.GetUserId();
            var cartInDB = await _unitOfWork.CartRepository.GetCartById(id);
            if (cartInDB == null){
                return BadRequest("Cart doesn't exist");
            }
            if (user != cartInDB.ClientId){
                return Unauthorized("You don't have permission to delete cart items");
            }
            _unitOfWork.CartRepository.DeleteCart(cartInDB);
            if (await _unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to delete cart of this vendor");
        }
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<Cart>> UpdateQuantityCart (CartItemDTO cartItemDTo){
            var cartInDB = await _unitOfWork.CartRepository.GetCartById(cartItemDTo.CartId);
            if (cartInDB == null) return BadRequest("Cart not found");
            var cartItem = _mapper.Map<CartItem>(cartItemDTo);
            if (await _unitOfWork.CartItemRepository.UpdateCartItem(cartItem)) return NoContent();
            return BadRequest("Failed to update cart item");
        }
        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> ClearCart(string clientName)
        {
            var user = User.GetUserName();
            if (!user.Equals(clientName)) return Unauthorized("You do not have permission to clear cart");
            if (await _cartServices.ClearCart(clientName)) return NoContent();
            return BadRequest("Something went wrong");
        }
    }
}