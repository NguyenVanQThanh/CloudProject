using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IPhotoService _photoService, IMapper _mapper, IUnitOfWork unitOfWork) : BaseApiController
    {
        // [AllowAnonymous]
        // [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDTOs>>> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUser = await unitOfWork.UserRepository.GetUserByUserName(User.GetUserName());
            userParams.CurrentUserName = currentUser.UserName;
            if (string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender = currentUser.Gender == "male"? "female" : "male";
            }
            var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize,users.TotalCount, users.TotalPages));
            return Ok(users);
        }
        // [HttpGet("{id}")]
        // public async Task<ActionResult<AppUser>> GetUser(int id){
        //     var user = await unitOfWork.UserRepository.GetByIdAsync(id);
        //     if (user == null){
        //         return NotFound();
        //     }
        //     return Ok(user);
        // }
        // [Authorize(Roles = "Member")]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTOs>> GetUserByUserName(string username)
        {
            var currentUsername = User.GetUserName();
            return await unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser : currentUsername == username);
        }
        [HttpPut]
        public async Task<ActionResult<MemberDTOs>> UpdateUser(MemberUpdateDto memberUpdateDTO)
        {
            var username = User.GetUserName();
            var user = await unitOfWork.UserRepository.GetUserByUserName(username);

            if (user == null) return NotFound();
            _mapper.Map(memberUpdateDTO, user);
            if (await unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to update user");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhotoAsync(IFormFile file)
        {
            var user = await unitOfWork.UserRepository.GetUserByUserName(User.GetUserName());
            if (user == null) return NotFound("User not found");
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            user.Photos.Add(photo);
            if (await unitOfWork.Complete())
            {
                return CreatedAtAction(nameof(GetUserByUserName),
                 new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem saving photo");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await unitOfWork.UserRepository.GetUserByUserName(User.GetUserName());
            if (user == null) return NotFound("User not found");
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound("Photo not found");
            if (photo.IsMain) return BadRequest("This is already the main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to set main photo");
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            {
                var user = await unitOfWork.UserRepository.GetUserByUserName(User.GetUserName());
                var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
                if (photo == null || photo.IsMain) return BadRequest("You cannot delete your main photo");
                if (photo.PublicId != null)
                {
                    var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                    if (result.Error != null) return BadRequest(result.Error.Message);
                }
                user.Photos.Remove(photo);
                if (await unitOfWork.Complete()) return Ok();
                return BadRequest("Failed to delete photo");
            }
        }
    }
}