using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PhotoService(IOptions<CloudinarySettings> cloudinaryConfig, IUnitOfWork unitOfWork, IMapper mapper)
        {
            // setup the Clpudinary Account
            Account acc = new Account
            (
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                // create using scope so the resources get disposed once the scope is finished
                await using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        // if the image is too big, tranform the image so just the face of the person is croped
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    // once the photo is uploaded, we should have the response back in this uploadResults
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string piblicId)
        {
            var deleteParams = new DeletionParams(piblicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }

        public async Task<PhotoCommentUserDto> GetPhCommentsById(int photoId)
        {
            var photoCommets = await _unitOfWork.PhotoRepository.GetPhotoComments(photoId);
            var photoCommetsCleaned = _mapper.Map<Photo, PhotoCommentUser>(photoCommets);
            // cleans out thr User
            var dtoUser = _mapper.Map<List<PhCommentCreator>, List<PhCommentCreatorDto>>(photoCommetsCleaned.PhCommentCreators);
            // manually mapped, see 
            //PhotoCommentUserDto photoCommentUserDto = new PhotoCommentUserDto {
            //    PhCommentCreatorDtos = dtoUser,
            //    Id = photoCommetsCleaned.Id,
            //    DateCreated = photoCommetsCleaned.DateCreated,
            //    Url = photoCommetsCleaned.Url,
            //    UserDto = photoCommetsCleaned.UserDto               
            //};
            PhotoCommentUserDto photoDto = _mapper.Map<PhotoCommentUser, PhotoCommentUserDto>(photoCommetsCleaned);

            return photoDto;
        }

        public async Task<List<PhotoCommentUser>> GetPhotoCommentsAndUsers()
        {
            var photosWithCom = await _unitOfWork.PhotoRepository.PhotosWithComments();
            var photosCleaned = _mapper.Map<List<Photo>, List<PhotoCommentUser>>(photosWithCom);

            return photosCleaned;
        }

        public async Task<List<Photo>> GetPhotosAndUsers()
        {
            List<Photo> photos = await _unitOfWork.PhotoRepository.GetAllPhotos();

            return photos; 
        }

        public async Task<List<PhotoCommentUserDto>> GetAllPhotosWithComments()
        {
            List<Photo> photos = await _unitOfWork.PhotoRepository.GetAllPhWithComAndUsr();
            List<PhotoCommentUser> cleaned = _mapper.Map<List<Photo>, List<PhotoCommentUser>>(photos);
            List<PhotoCommentUserDto> userDto = _mapper.Map<List<PhotoCommentUser>, List<PhotoCommentUserDto>>(cleaned);

            return userDto;
        }

        public async Task<bool> AddCommentForPhoto(PhotoComment photoComment)
        {
            _unitOfWork.PhotoCommentsRepository.AddComment(photoComment);
            if (await _unitOfWork.Complete())
            {
                return true;
            }
            return false;
        }

        public async Task<PhCommentCreatorDto> GetPhCommentAuthorById(int photoId, int userId, string comment)
        {
            var phCommentAndUser = await _unitOfWork.PhotoCommentsRepository.GetPhCommentAndUser(photoId, userId, comment);

            var phComUserDto = _mapper.Map<PhotoComment, PhCommentCreatorDto>(phCommentAndUser);

            return phComUserDto;
        }

        public async Task<PhCommentCreatorDto> EditPhotoComment(int photoId, string commentEdited, string commentOriginal)
        {
            if (commentOriginal != commentEdited)
            {
                PhotoComment photoCommentForEdit = await _unitOfWork.PhotoCommentsRepository.GetPhotoComment(photoId, commentOriginal);
                if (photoCommentForEdit == null)
                {
                    return null;
                }
                photoCommentForEdit.Comment = commentEdited;
               
                _unitOfWork.PhotoCommentsRepository.Update(photoCommentForEdit);
                if (await _unitOfWork.Complete())
                {
                    PhCommentCreatorDto phCommentCreatorDto = _mapper.Map<PhotoComment, PhCommentCreatorDto>(photoCommentForEdit);
                    return phCommentCreatorDto;
                }
            }
            
            
            return null;
        }

        public async Task<bool> DeletePhotoComment(int photoId, int userId)
        {
            if (await _unitOfWork.PhotoCommentsRepository.DeletePhotoComment(photoId, userId))
            {
                return await _unitOfWork.Complete();    
            }
            return false;
        }

        public async Task<PhotoLikeDto> LikePhoto(int photoId, int userId)
        {
            PhotoLikes photoLike = new PhotoLikes { PhotoId = photoId, AppUserId = userId };
            await _unitOfWork.PhotoRepository.LikePhoto(photoLike);
            if (await _unitOfWork.Complete())
            {
                AppUser user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
                photoLike.AppUser = user;
                PhotoLikeDto photoLikeDto = _mapper.Map<PhotoLikes, PhotoLikeDto>(photoLike);
                return photoLikeDto;
            }
            return null;
        }

        public async Task<bool> UnlikePhoto(int photoId, int userId)
        {
            PhotoLikes photoLikes = new PhotoLikes { PhotoId = photoId, AppUserId = userId };
            _unitOfWork.PhotoRepository.DeleteLike(photoLikes);
            if (await _unitOfWork. Complete())
            {
                return true;
            }
            return false;
        }

        public async Task<List<PhotoLikeDto>> GetLikesByPhotoId(int photoId)
        {
            var likes = await _unitOfWork.PhotoLikesRepository.GetPhotoLikesByPhotoId(photoId);
            List<PhotoLikeDto> PhotoLikeDto = _mapper.Map<List<PhotoLikes>, List<PhotoLikeDto>>(likes);

            return PhotoLikeDto;
        }

        public async Task<List<int>> LikedPhotosByUser(int userId)
        {
            List<int> likedPhtosId = await _unitOfWork.PhotoLikesRepository.GetPhotoLikesByUserId(userId);
            return likedPhtosId;
        }
    }
   
}
