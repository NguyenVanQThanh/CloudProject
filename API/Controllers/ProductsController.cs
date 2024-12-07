using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductsController(IUnitOfWork _unitOfWork, IMapper _mapper, IAzureBlobService _blobService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTOs>>> GetProducts([FromQuery] ProductParams productParams)
        {
            var products = await _unitOfWork.ProductRepository.GetAllProducts(productParams);
            products.PageSize = 12;
            Response.AddPaginationHeader(new PaginationHeader(products.CurrentPage,productParams.PageSize,products.TotalCount, products.TotalPages));
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
        [Authorize(Policy ="RequireVendorRole")]
        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromForm] CreateProductDTO createProductDTO){
            string imageUrl = null;
            if (createProductDTO.Image != null){
                var fileName = Path.GetFileName(createProductDTO.Image.FileName);
                // var fileName = Guid.NewGuid().ToString().Substring(0, 10) + Path.GetExtension(getFile);

                using (var stream = createProductDTO.Image.OpenReadStream()){
                    imageUrl = await _blobService.UploadFileAsync(stream, fileName);
                }
            }
            var product = new Product{
                Name = createProductDTO.Name,
                Description = createProductDTO.Description,
                Price = createProductDTO.Price,
                Quantity = createProductDTO.Quantity,
                ImageUrl = imageUrl,
                Vendor = await _unitOfWork.UserRepository.GetUserByUserName(User.GetUserName()),
                Category = await _unitOfWork.CategoryRepository.GetByIdAsync(createProductDTO.CategoryId),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true
            };
            _unitOfWork.ProductRepository.AddProduct(product);
            if (await _unitOfWork.Complete()){
                var productDTO = _mapper.Map<ProductDTOs>(product);
                return Ok(productDTO);
            }
            return BadRequest("Fail to create product");
        }
    }
}