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
using Microsoft.IdentityModel.Tokens;

namespace API.Data.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public CartRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddCart(Cart cart)
        {
            _context.Carts.Add(cart);
            _context.SaveChanges();
        }

        public void DeleteAllCartByClientNames(string clientName)
        {
            var listCartInDb = _context.Carts.Where(c => c.Client.UserName == clientName).ToList();
            if (listCartInDb.IsNullOrEmpty())
            _context.Carts.RemoveRange(listCartInDb);
        }

        public void DeleteCart(Cart cart)
        {
            _context.Carts.Remove(cart);
        }
        
        public async Task<PagedList<CartDTOs>> GetCartByParam(CartSearchParam cartSearchParam)
        {
            var cart = _context.Carts.AsQueryable();
            cart = _context.Carts.Where(c=>c.Client.UserName == cartSearchParam.ClientName)
                                .Include(c=>c.Vendor)
                                .Include(c=>c.Client)
                                .Include(c=>c.CartItems);
            if (!string.IsNullOrEmpty(cartSearchParam.VendorName)){
                cart = cart.Where(c=>c.Vendor.UserName == cartSearchParam.VendorName);
            }
            var query = cart.ProjectTo<CartDTOs>(_mapper.ConfigurationProvider);
            return await PagedList<CartDTOs>.CreateAsync(query, cartSearchParam.PageNumber, cartSearchParam.PageSize);
        }

        public async Task<Cart> GetCartById(int cartId)
        {
            return await _context.Carts
            .Include(c=>c.Vendor)
            .Include(c=>c.Client)
            .Include(c=>c.CartItems)
            .FirstOrDefaultAsync(c=>c.Id == cartId);
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
            if  (cartInDb == null){
                return false;
            }
            _context.Carts.Update(cart);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ICollection<Cart>> GetCartByClient(string clientName)
        {
            return await _context.Carts.Where(c=>c.Client.UserName == clientName)
                        .Include(c=>c.Vendor)
                        .Include(c=>c.CartItems)
                        .ToListAsync();
        }
    }
}