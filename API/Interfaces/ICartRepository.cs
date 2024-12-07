using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ICartRepository
    {
       Task<PagedList<CartDTOs>> GetCartByParam(CartSearchParam cartSearchParam);
       Task<ICollection<Cart>> GetCartByClient(string clientName);
       Task<Cart> GetCartById (int cartId);
       void AddCart(Cart cart);
       Task<bool> UpdateCart(Cart cart);
       void DeleteCart(Cart cart);
       void DeleteAllCartByClientNames(string clientName);
       Task<Cart> GetCartDetail(string clientName, string vendorName); 
    }
}