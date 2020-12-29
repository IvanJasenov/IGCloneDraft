using API.DTOs;
using API.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PhotoMappingProfiles : Profile
    {
        public PhotoMappingProfiles()
        {
            CreateMap<Photo, PhotoDto>()
                .ForMember(dest => dest.PhotoCommentsDto, opt => {
                    // manually select/project properties
                    opt.MapFrom(src => src.PhotoComments
                                          .Select(el => new PhotoCommentDto { 
                                              AppUserId = el.AppUserId, 
                                              DateCreated = el.DateCreated, 
                                              PhotoComment = el.Comment, 
                                              PhotoId = el.PhotoId,
                                              CommentId = el.Id,
                                              PhotoComentatoUsername = el.AppUser.UserName})); 
                })
                .ForMember(dest => dest.PhotoLikesDto, opt => {
                    // I set mappings for PhotoLikes => PhotoLikesDto in PhotoLikeMapperProfiles.cs
                    opt.MapFrom(src => src.PhotoLikes);
                })
                .ReverseMap();

           

            CreateMap<Photo, PhotosToApproveDto>()
                .ForMember(dest => dest.Username, opt =>
                {
                    opt.MapFrom(src => src.AppUser.UserName);
                }).ReverseMap();

            //// for Instagram 
            CreateMap<Photo, PhotoCommentUser>()
                .ForMember(dest => dest.UserDto, opt =>
                {
                    opt.MapFrom(src => src.AppUser);
                })
                .ForMember(dest => dest.Url, opt =>
                {
                    opt.MapFrom(src => src.Url);
                }) .ForMember(dest => dest.Country, opt =>
                {
                    opt.MapFrom(src => src.AppUser.Country);
                })
                //.ForMember(dest => dest.PhotoComments, opt =>
                //{
                //    opt.MapFrom(src => src.PhotoComments.Select(el => el.Comment).ToList());
                //})
                .ForMember(dest => dest.PhCommentCreators, opt =>
                {
                    // manually select/project properties
                    opt.MapFrom(src => src.PhotoComments
                                          .Select(el => new PhCommentCreator {
                                              AppUser = el.AppUser, 
                                              Comment = el.Comment, 
                                              DateCreated = el.DateCreated, }));
                });

            // For Instagram
            CreateMap<PhotoComment, PhCommentCreatorDto>()
                .ForMember(dest => dest.CommentCreator, opt => {
                    opt.MapFrom(src => src.AppUser); 
                    // in this case, mapping is from AppUser -> UserDto. We already have this mapping set and the 
                    // AutoMapper will look into previous mappings and figure it out
                });

            // aditional mapping to return UserDto from nested type
            CreateMap<PhCommentCreator, PhCommentCreatorDto>()
               .ForPath(dest => dest.CommentCreator.Gender, opt => {
                    opt.MapFrom(src => src.AppUser.Gender);
                }).ForPath(dest => dest.CommentCreator.KnownAs, opt => {
                    opt.MapFrom(src => src.AppUser.KnownAs);
                }).ForPath(dest => dest.CommentCreator.Username, opt => {
                    opt.MapFrom(src => src.AppUser.UserName);
                }).ForPath(dest => dest.CommentCreator.Country, opt => {
                    opt.MapFrom(src => src.AppUser.Country);
                }).ForPath(dest => dest.CommentCreator.PhotoUrl, opt => {
                    opt.MapFrom(src => src.AppUser.Photos.FirstOrDefault(el => el.IsMain).Url);
                });

            // this is not finished, mapping in service is done manually
            CreateMap<PhotoCommentUser, PhotoCommentUserDto>()
                .ForMember(dest => dest.PhCommentCreatorDtos, opt =>
                {
                    opt.MapFrom(src => (src.PhCommentCreators));
                }) .ForMember(dest => dest.PhotoOwner, opt =>
                {
                    opt.MapFrom(src => (src.UserDto));
                });
        }
    }
}
