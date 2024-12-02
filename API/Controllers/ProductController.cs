using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductController(IUnitOfWork _unitOfWork, IMapper _mapper) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTOs>>> GetProducts([FromQuery] ProductParams productParams)
        {
            var products = await _unitOfWork.ProductRepository.GetAllProducts(productParams);
            Response.AddPaginationHeader(new PaginationHeader(products.CurrentPage, products.PageSize, products.TotalPages));
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTOs>> GetProductsById(int id){
            var product = await _unitOfWork.ProductRepository.GetProductById(id);
            if (product == null){
                return NotFound("Product not found");
            }
            var result = _mapper.Map<ProductDTOs>(product);
            return Ok(result);        
        }
        [Authorize(Policy ="RequireVendorRole")]
        [HttpPut]
        public async Task<ActionResult<ProductDTOs>> UpdateProduct(ProductDTOs productDTOs){
            if (!productDTOs.Vendor.Equals(User.GetUserName())){
                return Unauthorized("You are not authorized to update this product");
            }
            var product = await _unitOfWork.ProductRepository.GetProductById(productDTOs.Id);
            if (product == null){
                return NotFound("Product not found");
            }
            _mapper.Map(productDTOs, product);
            if (await _unitOfWork.Complete()){
                return NoContent();
            }
            return BadRequest("Fail to update product");
        }
    }
}