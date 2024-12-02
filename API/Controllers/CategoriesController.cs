using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CategoriesController(IUnitOfWork _unitOfWork, IMapper _mapper) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ICollection<Category>>> GetCategories(){
            var categories = await _unitOfWork.CategoryRepository.GetCategoriesAsync();
            return Ok(categories);
        }
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] CategoryDTO categoryDTO){
            var category = _mapper.Map<Category>(categoryDTO);
            _unitOfWork.CategoryRepository.Add(category);
            await _unitOfWork.Complete();
            return Ok(category);
        }
        [HttpPut]
        public async Task<ActionResult<Category>> UpdateCategory([FromBody] CategoryDTO categoryDTO){
            var categoryInDB = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryDTO.Id);
            if (categoryInDB == null) return BadRequest("Category not found");
            var updatedCategory = _mapper.Map<Category>(categoryDTO);
            _unitOfWork.CategoryRepository.UpdateCategory(updatedCategory);
            if (await _unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to update category");
        }
    }
}