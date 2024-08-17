using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task<AppUser> GetByIdAsync(int id);
        Task<AppUser> GetUserByUserName(string userName);
        Task<PagedList<MemberDTOs>> GetMembersAsync(UserParams userParams);
        Task<MemberDTOs> GetMemberAsync(string username);
    }

    public class UserParms
    {
    }
}