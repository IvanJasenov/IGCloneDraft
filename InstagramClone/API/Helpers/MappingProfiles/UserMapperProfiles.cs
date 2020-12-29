using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class UserMapperProfiles : Profile
    {
        public UserMapperProfiles()
        {
            CreateMap<AppUser, UserDto>()
              .ForMember(dest => dest.PhotoUrl, opt =>
              {
                  opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
              });


            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.Age, opt =>
                {
                    //opt.MapFrom(src => src.GetAge());
                    opt.MapFrom(src => src.DateOfBirth.CalculateAge()); // use the extension directly
                })
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    // query Photos from the AppUser
                    opt.MapFrom(src => src.Photos.FirstOrDefault(el => el.IsMain).Url);
                })
                // this mapping is for Instagram photo list, where I need to load the additional info
                .ForPath(dest => dest.PhotosDto, opt => {
                    opt.MapFrom(src => src.Photos); 
                })
                .ReverseMap();



            CreateMap<MemberUpdateDto, AppUser>();

            CreateMap<RegisterDto, AppUser>();

            CreateMap<AppRole, RoleDto>().ReverseMap();
        }
    }
}
