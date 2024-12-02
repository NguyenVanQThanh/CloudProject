using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public DateOnly DateOfBirth { get; set; }
        public string? KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string? Gender { get; set; }
        public string Introduction { get; set; } = "";
        public string LookingFor { get; set; } = "";
        public string Interests { get; set; } = "";
        public string? City { get; set; }
        public string? Country { get; set; }
        public decimal? Amount { get; set; }
        public List<Photo> Photos { get; set; } = [];
        public List<UserLike>? LikeByUsers { get; set; }
        public List<UserLike>? LikedUsers { get; set; }
        public List<Message>? MessagesSent { get; set; }
        public List<Message>? MessagesReceived { get; set; }
        public List<Order>? OrderClients { get; set; }
        public List<Order>? OrderVendors { get; set; }
        public List<Cart>? CartClients{ get; set; }
        public List<Cart>? CartVendors { get; set; }
        public List<Product>? VendorProducts { get; set; }
        public ICollection<AppUserRole>? UserRoles { get; set; }
    }
}