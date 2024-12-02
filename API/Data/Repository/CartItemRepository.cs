using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly DataContext _context;
        public CartItemRepository(DataContext context)
        {
            _context = context;
        }
        public void AddCartItem(CartItem item)
        {
            _context.CartItems.Add(item);
            
        }

        public async Task<bool> DeleteCartItem(int productId, int cartId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.ProductId == productId && ci.CartId == cartId);
            if (cartItem!=null)
            {
                _context.CartItems.Remove(cartItem);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<CartItem> GetCartItemsAsync(int cartId, int productId)
        {
            return await _context.CartItems.FindAsync(cartId, productId);
        }

        public async Task<bool> UpdateCartItem(CartItem item)
        {
            var cartInDb = await _context.CartItems.FirstOrDefaultAsync(ci=>ci.CartId ==item.CartId && ci.ProductId == item.ProductId);
            if (cartInDb == null) return false;
             _context.Update(item);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}