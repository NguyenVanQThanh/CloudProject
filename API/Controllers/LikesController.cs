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
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository){
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }
        [HttpPost("{id}")]
        public async Task<ActionResult> AddLike(int id){
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetByIdAsync(id);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);
            if (likedUser == null) return NotFound();
            if (sourceUser.Id == id) return BadRequest("You cannot like yourself");
            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null) return BadRequest("You already like this user");
            userLike = new UserLike{
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            if (await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to like user");
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTOs>>> GetLikes([FromQuery] LikesParams likesParams){
            likesParams.UserId = User.GetUserId();
            var users = await _likesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalPages));
            return Ok(users);
        }
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds(){
            return Ok(await _likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
        }
        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId){
            var sourceUserId = User.GetUserId();
            if (sourceUserId == targetUserId) return BadRequest("You cannot like yourself");
            var existingLike = await _likesRepository.GetUserLike(sourceUserId, targetUserId);
            if (existingLike == null){
                var Like = new UserLike{
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId
                };
                _likesRepository.AddLike(Like);
            } else {
                _likesRepository.DeleteLike(existingLike);
            }
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to toggle like");
        }
    }
}