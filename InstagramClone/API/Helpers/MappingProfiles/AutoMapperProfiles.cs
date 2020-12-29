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
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
           
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(el => el.IsMain).Url);
                })
                .ForMember(dest => dest.RecipientPhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(el => el.IsMain).Url);
                });
            
            
        }
    }
}
