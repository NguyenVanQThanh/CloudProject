using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Data.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;
        public CartRepository(DataContext context)
        {
            _context = context;
        }
        public void AddCart(Cart cart)
        {
            _context.Carts.Add(cart);
            _context.SaveChanges();
        }

        public async Task<bool> DeleteAllCartByClientNames(string clientName)
        {
            var listCartInDb = await _context.Carts.Where(c => c.Client.UserName == clientName).ToListAsync();
            if (listCartInDb.IsNullOrEmpty()) return false;
            _context.Carts.RemoveRange(listCartInDb);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCartByClientName(string clientName, string vendorName)
        {
            var cartInDb = await _context.Carts.Where(c => c.Client.UserName == clientName && c.Vendor.UserName == vendorName).FirstOrDefaultAsync();
            if (cartInDb != null){
                _context.Carts.Remove(cartInDb);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
        
        public async Task<ICollection<Cart>> GetCartByClientName(string userName)
        {
            return await _context.Carts.Where(c => c.Client.UserName == userName)
                            .Include(c=>c.Vendor)
                            .Include(c=>c.CartItems)
                            .ToListAsync();
        }

        public async Task<Cart> GetCartDetail(string clientName, string vendorName)
        {
            return await _context.Carts.Where(c=>c.Client.UserName == clientName && c.Vendor.UserName == vendorName)
                        .Include(c=>c.Vendor)
                        .Include(c=>c.CartItems)
                        .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateCart(Cart cart)
        {
            var cartInDb = await _context.Carts.FirstOrDefaultAsync(c=>c.Id == cart.Id);
            if  (cartInDb == null) return false;
            _context.Carts.Update(cart);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}