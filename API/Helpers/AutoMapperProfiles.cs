using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(){
            CreateMap<AppUser, MemberDTOs>()
            .ForMember(dest => dest.PhotoUrl,
            opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x=>x.IsMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDTO, AppUser>();
            CreateMap<Message, MessageDto>()
                .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos
                    .FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos
                    .FirstOrDefault(x => x.IsMain).Url));
            CreateMap<DateTime, DateTime>().ConvertUsing(d=>DateTime.SpecifyKind(d, DateTimeKind.Utc));
            CreateMap<DateTime?,DateTime?>().ConvertUsing(d=> d.HasValue ? DateTime.SpecifyKind(d.Value,DateTimeKind.Utc) : null );

            CreateMap<Product, ProductDTOs>()
            .ForMember(p=> p.CategoryName,opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(p=> p.Vendor, opt=> opt.MapFrom(src => src.Vendor.UserName));
            CreateMap<Category, CategoryDTO>();
            CreateMap<Order, OrderDTO>()
            .ForMember(o=>o.Status, opt => opt.MapFrom(src => nameof(src.Status)))
            .ForMember(o=>o.Payment, opt => opt.MapFrom(src => nameof(src.Payment)));
            CreateMap<Cart, CartDTOs>()
            .ForMember(c=>c.VendorName, opt=> opt.MapFrom(src => src.Vendor.UserName))
            .ForMember(c=>c.ClientName, opt=> opt.MapFrom(src => src.Client.UserName))
            .ForMember(c=>c.TotalPrice, opt=> opt.MapFrom(src => src.CartItems.Sum(ci=>ci.Quantity*ci.Product.Price)));
            CreateMap<CartItem, CartItemDTO>()
            .ForMember(c=>c.ProductName,opt=> opt.MapFrom(src => src.Product.Name))
            .ForMember(c=>c.Price, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(c=>c.QuantityInStock,opt=> opt.MapFrom(src => src.Product.Quantity));
            ;
        }
    }
}