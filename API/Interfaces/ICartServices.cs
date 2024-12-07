using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;

namespace API.Interfaces
{
    public interface ICartServices
    {
        Task<bool> AddToCart(AddToCartDTO addToCartDTO);
        Task<bool> RemoveFromCart(int productId, int cartId);
        Task<bool> UpdateQuantity(CartItemDTO cartItemDTO);
        Task<bool> ClearCart(string clientName);
    }
}