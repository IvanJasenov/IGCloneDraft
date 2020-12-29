using API.DTOs;
using API.Entities;
using API.Helpers;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        Task<DeletionResult> DeletePhotoAsync(string piblicId);

        Task<List<Photo>> GetPhotosAndUsers();

        #region PhotoComments

        Task<List<PhotoCommentUser>> GetPhotoCommentsAndUsers();

        Task<PhotoCommentUserDto> GetPhCommentsById(int photoId);

        Task<PhCommentCreatorDto> GetPhCommentAuthorById(int photoId, int userId, string comment);

        Task<List<PhotoCommentUserDto>> GetAllPhotosWithComments();

        Task<bool> AddCommentForPhoto(PhotoComment photoComment);

        Task<PhCommentCreatorDto> EditPhotoComment(int photoIs, string commentEdited, string commentOriginal);

        Task<bool> DeletePhotoComment(int photoCommentId, int userId);

        #endregion

        #region PhotoLikes

        Task<PhotoLikeDto> LikePhoto(int photoId, int userId);

        Task<bool> UnlikePhoto(int photoId, int userId);

        Task<List<PhotoLikeDto>> GetLikesByPhotoId(int photoId);

        Task<List<int>> LikedPhotosByUser(int userId);
        #endregion


    }
}
