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
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
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
            CreateMap<Order, OrderDTO>();
        }
    }
}