using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
    {
        // private readonly ILikesRepository unitOfWork.LikesRepository;

        [HttpPost("{id}")]
        public async Task<ActionResult> AddLike(int id){
            var sourceUserId = User.GetUserId();
            var likedUser = await unitOfWork.UserRepository.GetByIdAsync(id);
            var sourceUser = await unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);
            if (likedUser == null) return NotFound();
            if (sourceUser.Id == id) return BadRequest("You cannot like yourself");
            var userLike = await unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null) return BadRequest("You already like this user");
            userLike = new UserLike{
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            if (await unitOfWork.Complete()) return Ok();
            return BadRequest("Failed to like user");
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTOs>>> GetLikes([FromQuery] LikesParams likesParams){
            likesParams.UserId = User.GetUserId();
            var users = await unitOfWork.LikesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalPages));
            return Ok(users);
        }
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds(){
            return Ok(await unitOfWork.LikesRepository.GetCurrentUserLikeIds(User.GetUserId()));
        }
        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId){
            var sourceUserId = User.GetUserId();
            if (sourceUserId == targetUserId) return BadRequest("You cannot like yourself");
            var existingLike = await unitOfWork.LikesRepository.GetUserLike(sourceUserId, targetUserId);
            if (existingLike == null){
                var Like = new UserLike{
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId
                };
                unitOfWork.LikesRepository.AddLike(Like);
            } else {
                unitOfWork.LikesRepository.DeleteLike(existingLike);
            }
            if (await unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to toggle like");
        }
    }
}