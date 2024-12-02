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
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ProductRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
        }

        public async Task<bool> DisabledProduct(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p=>p.Id == productId);
            if (product == null || product.Status == false){
                return false;
            }
            product.Status = true;
            _context.Products.Update(product);
            return true;
        }

        public async Task<PagedList<ProductDTOs>> GetAllProducts(ProductParams productParams)
        {
            var products = _context.Products
                        .OrderByDescending(x=>x.Status)
                        .Include(p=>p.Category)
                        .Include(p=>p.Vendor)
                        .AsQueryable();
            if (!string.IsNullOrEmpty(productParams.ProductName)){
                products = products.Where(p=>p.Name.Contains(productParams.ProductName));
            }
            if (!productParams.CategoryName.IsNullOrEmpty()){
                products = products.Where(p=>productParams.CategoryName.Contains(p.Category.Name));
            }
            if (string.IsNullOrEmpty(productParams.VendorName)){
                products = products.Where(p=>p.Vendor.UserName == productParams.VendorName);
            }
            if (productParams.Status.HasValue){
                products = products.Where(p=>p.Status == productParams.Status.Value);
            }
            if (productParams.MinPrice.HasValue){
                products = products.Where(p=>p.Price >= productParams.MinPrice.Value);
            }
            if (productParams.MaxPrice.HasValue){
                products = products.Where(p=>p.Price <= productParams.MaxPrice.Value);
            }
            var query = products.ProjectTo<ProductDTOs>(_mapper.ConfigurationProvider); 
            return await PagedList<ProductDTOs>.CreateAsync(query, productParams.PageNumber, productParams.PageSize);

        }

        public async Task<Product> GetProductById(int productId)
        {
            return await _context.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
        }
    }
}