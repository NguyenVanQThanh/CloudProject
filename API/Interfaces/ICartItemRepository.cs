using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItem> GetCartItemsAsync(int cartId, int productId);
        void AddCartItem(CartItem item);
        Task<bool> DeleteCartItem(int productId, int cartId);
        Task<bool> UpdateCartItem(CartItem item);
    }
}