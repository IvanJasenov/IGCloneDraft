using API.DTOs;
using API.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers.MappingProfiles
{
    public class PhotoLikeMapperProfiles : Profile
    {
        public PhotoLikeMapperProfiles()
        {
            CreateMap<PhotoLikes, PhotoLikeDto>()
                .ForMember(dest => dest.UserDto, opt => {
                    opt.MapFrom(src => src.AppUser);
                }).ForMember(dest => dest.UserId, opt => {
                    opt.MapFrom(src => src.AppUser.Id);
                });
        }
    }
}
