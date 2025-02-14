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
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public LikesRepository(DataContext context, IMapper mapper){
            _context = context;
            _mapper = mapper;
        }

        public void AddLike(UserLike like)
        {
            _context.Likes.Add(like);
        }

        public void DeleteLike(UserLike like)
        {
            _context.Likes.Remove(like);
        }

        public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
        {
            return await _context.Likes
            .Where( x=> x.SourceUserId == currentUserId)
            .Select( x=> x.TargetUserId)
            .ToListAsync();
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<MemberDTOs>> GetUserLikes(LikesParams likesParams)
        {
            var likes = _context.Likes.AsQueryable();
            IQueryable<MemberDTOs> query;
            switch (likesParams.Predicate){
                case "liked":
                    query = likes
                    .Where(x => x.SourceUserId == likesParams.UserId)
                    .Select(x => x.TargetUser)
                    .ProjectTo<MemberDTOs>(_mapper.ConfigurationProvider);
                    break;
                case "likedBy": 
                    query = likes
                    .Where(x => x.TargetUserId == likesParams.UserId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDTOs>(_mapper.ConfigurationProvider);
                    break;
                default: 
                    var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);
                    query =  likes.Where(x => x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId))
                    .Select(x=>x.SourceUser)
                    .ProjectTo<MemberDTOs>(_mapper.ConfigurationProvider);
                    break;
                    
            }
            return await PagedList<MemberDTOs>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
        }
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
            .Include( x=>x.LikedUsers)
            .FirstOrDefaultAsync(x=>x.Id == userId);
        }
    }
}