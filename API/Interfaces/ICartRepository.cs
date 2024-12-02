using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface ICartRepository
    {
       Task<ICollection<Cart>> GetCartByClientName(string userName);
       void AddCart(Cart cart);
       Task<bool> UpdateCart(Cart cart);
       Task<bool> DeleteCartByClientName(string clientName, string vendorName); 
       Task<bool> DeleteAllCartByClientNames(string clientName);
       Task<Cart> GetCartDetail(string clientName, string vendorName); 
    }
}