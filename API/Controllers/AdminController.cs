using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.Repository;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService,
                                IOrderServices _orderServices) : BaseApiController
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles(){
            var users = await userManager.Users
                .OrderBy(x=>x.UserName)
                .Select(x=> new {
                    x.Id,
                    UserName = x.UserName,
                    Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
                }).ToListAsync();
            return Ok(users);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, string roles){
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");
            var selectedRoles = roles.Split(",").ToArray();
            var user = await userManager.FindByNameAsync(username);
            if (user == null) return BadRequest("User not found");
            var userRoles = await userManager.GetRolesAsync(user);
            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Failed to add roles");
            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to remove from roles");
            return Ok(await userManager.GetRolesAsync(user));
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration(){
            var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();
            return Ok(photos);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("approve-photo/{photo-id}")]
        public async Task<ActionResult> ApprovePhoto(int photoId){
            var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if (photo == null) return BadRequest("Could not get photo from db");
            photo.IsApproved = true;
            var user = await unitOfWork.UserRepository.GetUserByPhotoId(photoId);
            if (user == null) return BadRequest("Could not get user from db");
            if (!user.Photos.Any(x=> x.IsMain)) photo.IsMain = true;
            await unitOfWork.Complete();
            return Ok();
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("reject-photo/{photo-id}")]
        public async Task<ActionResult> RejectPhoto(int photoId){
            var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if (photo == null) return BadRequest("Could not get photo from db");
            if (photo.PublicId != null){
                var result = await photoService.DeletePhotoAsync(photo.PublicId); 
                if (result.Result == "ok"){
                    unitOfWork.PhotoRepository.RemovePhoto(photo);
                }
            } else {
                unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
            await unitOfWork.Complete();
            return Ok();
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("orders/status/{id}")]
        public async Task<ActionResult> UpdateOrderStatus(int id,[FromBody]string status){
            try{
                if (await _orderServices.UpdateStatus(id, status)) return Ok("OrderStatus has been updated");
            }catch(Exception ex){
                return BadRequest(ex.Message);
            }
            return BadRequest("Failed to update order status");
        }
    }
}