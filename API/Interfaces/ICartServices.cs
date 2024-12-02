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
    }
}