using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedList<ProductDTOs>> GetAllProducts(ProductParams productParams);
        Task<Product> GetProductById(int productId);
        void AddProduct(Product product);
        Task<bool> DisabledProduct(int productId);
    }
}